using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using View.Model;
using View.Model.Services.View.Model.Services;

namespace View.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        private Contact _contact;
        private ContactSerializer _serializer;
        private SaveCommand _saveCommand;
        private LoadCommand _loadCommand;

        /// <summary>
        /// Команда для сохранения контакта
        /// </summary>
        public SaveCommand SaveCommand => _saveCommand;

        /// <summary>
        /// Команда для загрузки контакта
        /// </summary>
        public LoadCommand LoadCommand => _loadCommand;

        /// <summary>
        /// Имя контакта
        /// </summary>
        public string Name
        {
            get => _contact.Name;
            set
            {
                if (_contact.Name != value)
                {
                    _contact.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Номер телефона контакта
        /// </summary>
        public string PhoneNumber
        {
            get => _contact.PhoneNumber;
            set
            {
                if (_contact.PhoneNumber != value)
                {
                    _contact.PhoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Email контакта
        /// </summary>
        public string Email
        {
            get => _contact.Email;
            set
            {
                if (_contact.Email != value)
                {
                    _contact.Email = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Конструктор MainVM
        /// </summary>
        public MainVM()
        {
            _contact = new Contact();
            _serializer = new ContactSerializer();
            _saveCommand = new SaveCommand(this, _serializer);
            _loadCommand = new LoadCommand(this, _serializer);
        }

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged
        /// </summary>
        /// <param name="propertyName">Имя изменившегося свойства</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
