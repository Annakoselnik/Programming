using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contacts.Model;
using Contacts.Model.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Contacts.ViewModel
{
    public class MainVM : ObservableObject
    {
        private readonly ContactSerializer _serializer = new ContactSerializer();
        private ObservableCollection<Contact> _contacts;
        private ObservableCollection<Contact> _filteredContacts;
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
                if (SetProperty(ref _contacts, value))
                    UpdateFilteredContacts();
            }
        }

        public ObservableCollection<Contact> FilteredContacts
        {
            get => _filteredContacts;
            set => SetProperty(ref _filteredContacts, value);
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (SetProperty(ref _filterText, value ?? string.Empty))
                    UpdateFilteredContacts();
            }
        }

        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                if (_selectedContact == value) return;
                if (_currentMode != Mode.Normal && value != null) CancelOperation();
                _selectedContact = value;
                if (_currentMode == Mode.Normal)
                {
                    CurrentContact = _selectedContact?.Clone() ?? new Contact();
                    IsReadOnly = true;
                    ApplyVisibility = false;
                }
                OnPropertyChanged();
                (AddCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public Contact CurrentContact
        {
            get => _currentContact;
            set
            {
                if (SetProperty(ref _currentContact, value))
                {
                    OnPropertyChanged(nameof(CurrentName));
                    OnPropertyChanged(nameof(CurrentPhone));
                    OnPropertyChanged(nameof(CurrentEmail));
                }
            }
        }

        public string CurrentName
        {
            get => CurrentContact?.Name ?? string.Empty;
            set { if (CurrentContact != null && CurrentContact.Name != value) CurrentContact.Name = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string CurrentPhone
        {
            get => CurrentContact?.PhoneNumber ?? string.Empty;
            set { if (CurrentContact != null && CurrentContact.PhoneNumber != value) CurrentContact.PhoneNumber = value ?? string.Empty; OnPropertyChanged(); }
        }

        public string CurrentEmail
        {
            get => CurrentContact?.Email ?? string.Empty;
            set { if (CurrentContact != null && CurrentContact.Email != value) CurrentContact.Email = value ?? string.Empty; OnPropertyChanged(); }
        }

        public bool IsReadOnly { get => _isReadOnly; set => SetProperty(ref _isReadOnly, value); }
        public bool ApplyVisibility { get => _applyVisibility; set => SetProperty(ref _applyVisibility, value); }

        public bool IsEditingOrAdding
        {
            get => _isEditingOrAdding;
            set
            {
                if (SetProperty(ref _isEditingOrAdding, value))
                {
                    (AddCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }

        public MainVM()
        {
            var loaded = _serializer.Load();
            Contacts = new ObservableCollection<Contact>(loaded);
            if (Contacts.Any()) SelectedContact = Contacts[0];
            else CurrentContact = new Contact();

            AddCommand = new RelayCommand(StartAdd, () => !IsEditingOrAdding);
            EditCommand = new RelayCommand(StartEdit, () => !IsEditingOrAdding && SelectedContact != null);
            RemoveCommand = new RelayCommand(RemoveContact, () => !IsEditingOrAdding && SelectedContact != null);
            ApplyCommand = new RelayCommand(Apply, () => IsEditingOrAdding && !(CurrentContact?.HasErrors ?? true));
            CancelCommand = new RelayCommand(CancelOperation, () => IsEditingOrAdding);

            PropertyChanged += (snd, evt) =>
            {
                if (evt.PropertyName == nameof(CurrentContact) && CurrentContact != null)
                {
                    CurrentContact.ErrorsChanged += (s, e) => (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                }
            };
        }

        private void UpdateFilteredContacts()
        {
            if (Contacts == null) return;
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                FilteredContacts = new ObservableCollection<Contact>(Contacts);
            }
            else
            {
                var filtered = Contacts.Where(c => c.Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0);
                FilteredContacts = new ObservableCollection<Contact>(filtered);
            }
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
            if (!IsEditingOrAdding || (CurrentContact?.HasErrors ?? true)) return;
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
            if (SelectedContact == null && Contacts.Any()) SelectedContact = Contacts[0];
            UpdateFilteredContacts(); // обновляем отфильтрованный список
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
                int newIndex = index < Contacts.Count ? index : Contacts.Count - 1;
                SelectedContact = Contacts[newIndex];
            }
            UpdateFilteredContacts(); // обновляем отфильтрованный список
        }

        private void SaveToFile() => _serializer.Save(Contacts);
    }
}