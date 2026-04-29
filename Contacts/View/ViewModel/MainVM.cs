using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using View.Model;
using View.Model.Services;

namespace View.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        private readonly ContactSerializer _serializer;
        private ObservableCollection<Contact> _contacts;
        private ICollectionView _filteredContactsView;
        private Contact _selectedContact;
        private Contact _currentContact;
        private Contact _originalContact;
        private string _filterText = string.Empty;
        private bool _isReadOnly = true;
        private bool _isEditingOrAdding = false;
        private bool _applyVisibility = false;
        private Mode _currentMode = Mode.Normal;

        public enum Mode { Normal, Add, Edit }

        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set
            {
                _contacts = value;
                OnPropertyChanged();
                UpdateFilteredView();
            }
        }

        public ICollectionView FilteredContactsView
        {
            get => _filteredContactsView;
            set
            {
                _filteredContactsView = value;
                OnPropertyChanged();
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText != value)
                {
                    _filterText = value ?? string.Empty;
                    OnPropertyChanged();
                    FilteredContactsView?.Refresh();
                }
            }
        }

        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                if (_selectedContact != value)
                {
                    if (_currentMode != Mode.Normal && value != null)
                        CancelOperation();

                    _selectedContact = value;

                    if (_currentMode == Mode.Normal)
                    {
                        CurrentContact = _selectedContact?.Clone() ?? new Contact();
                        IsReadOnly = true;
                        ApplyVisibility = false;
                    }

                    OnPropertyChanged();
                    (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (RemoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public Contact CurrentContact
        {
            get => _currentContact;
            set
            {
                _currentContact = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentName));
                OnPropertyChanged(nameof(CurrentPhone));
                OnPropertyChanged(nameof(CurrentEmail));
            }
        }

        public string CurrentName
        {
            get => CurrentContact?.Name ?? string.Empty;
            set
            {
                if (CurrentContact != null && CurrentContact.Name != value)
                {
                    CurrentContact.Name = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentPhone
        {
            get => CurrentContact?.PhoneNumber ?? string.Empty;
            set
            {
                if (CurrentContact != null && CurrentContact.PhoneNumber != value)
                {
                    CurrentContact.PhoneNumber = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentEmail
        {
            get => CurrentContact?.Email ?? string.Empty;
            set
            {
                if (CurrentContact != null && CurrentContact.Email != value)
                {
                    CurrentContact.Email = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                OnPropertyChanged();
            }
        }

        public bool ApplyVisibility
        {
            get => _applyVisibility;
            set
            {
                _applyVisibility = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditingOrAdding
        {
            get => _isEditingOrAdding;
            set
            {
                _isEditingOrAdding = value;
                OnPropertyChanged();
                (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ApplyCommand { get; }

        public MainVM()
        {
            _serializer = new ContactSerializer();
            var loaded = _serializer.Load();
            Contacts = new ObservableCollection<Contact>(loaded);
            if (Contacts.Any())
                SelectedContact = Contacts[0];
            else
                CurrentContact = new Contact();

            AddCommand = new RelayCommand(_ => StartAdd(), _ => !IsEditingOrAdding);
            EditCommand = new RelayCommand(_ => StartEdit(), _ => !IsEditingOrAdding && SelectedContact != null);
            RemoveCommand = new RelayCommand(_ => RemoveContact(), _ => !IsEditingOrAdding && SelectedContact != null);
            ApplyCommand = new RelayCommand(_ => Apply(), _ => IsEditingOrAdding);
        }

        private void UpdateFilteredView()
        {
            FilteredContactsView = CollectionViewSource.GetDefaultView(Contacts);
            FilteredContactsView.Filter = contact =>
            {
                if (string.IsNullOrWhiteSpace(FilterText)) return true;
                return ((Contact)contact).Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
            };
        }

        private void StartAdd()
        {
            if (IsEditingOrAdding) return;
            CancelOperation();
            _currentMode = Mode.Add;
            IsEditingOrAdding = true;
            IsReadOnly = false;
            ApplyVisibility = true;
            SelectedContact = null;
            CurrentContact = new Contact();
        }

        private void StartEdit()
        {
            if (IsEditingOrAdding || SelectedContact == null) return;
            _originalContact = SelectedContact.Clone();
            _currentMode = Mode.Edit;
            IsEditingOrAdding = true;
            IsReadOnly = false;
            ApplyVisibility = true;
            CurrentContact = SelectedContact.Clone();
        }

        private void Apply()
        {
            if (!IsEditingOrAdding) return;

            if (_currentMode == Mode.Add && CurrentContact != null)
            {
                Contacts.Add(CurrentContact);
                SelectedContact = CurrentContact;
            }
            else if (_currentMode == Mode.Edit && SelectedContact != null && CurrentContact != null)
            {
                SelectedContact.Name = CurrentContact.Name;
                SelectedContact.PhoneNumber = CurrentContact.PhoneNumber;
                SelectedContact.Email = CurrentContact.Email;
            }

            SaveToFile();

            _currentMode = Mode.Normal;
            IsEditingOrAdding = false;
            IsReadOnly = true;
            ApplyVisibility = false;
            CurrentContact = SelectedContact?.Clone() ?? new Contact();
            if (SelectedContact == null && Contacts.Any())
                SelectedContact = Contacts[0];
        }

        private void CancelOperation()
        {
            if (_currentMode == Mode.Edit && _originalContact != null && SelectedContact != null)
            {
                SelectedContact.Name = _originalContact.Name;
                SelectedContact.PhoneNumber = _originalContact.PhoneNumber;
                SelectedContact.Email = _originalContact.Email;
            }
            _currentMode = Mode.Normal;
            IsEditingOrAdding = false;
            IsReadOnly = true;
            ApplyVisibility = false;
            CurrentContact = SelectedContact?.Clone() ?? new Contact();
        }

        private void RemoveContact()
        {
            if (SelectedContact == null || IsEditingOrAdding) return;
            int index = Contacts.IndexOf(SelectedContact);
            Contacts.Remove(SelectedContact);
            SaveToFile();

            if (Contacts.Count == 0)
            {
                SelectedContact = null;
                CurrentContact = new Contact();
            }
            else
            {
                int newIndex = (index < Contacts.Count) ? index : Contacts.Count - 1;
                SelectedContact = Contacts[newIndex];
            }
        }

        private void SaveToFile()
        {
            _serializer.Save(Contacts);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}