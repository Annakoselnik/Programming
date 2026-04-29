using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace View.Model
{
    /// <summary>Модель контакта с поддержкой уведомлений об изменении свойств.</summary>
    public class Contact : INotifyPropertyChanged
    {
        private string _name;
        private string _phoneNumber;
        private string _email;

        /// <summary>Имя контакта. Не может быть null (заменяется на пустую строку).</summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Номер телефона. Не может быть null (заменяется на пустую строку).</summary>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Электронная почта. Не может быть null (заменяется на пустую строку).</summary>
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Создаёт пустой контакт со всеми полями равными пустой строке.</summary>
        public Contact()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
            Email = string.Empty;
        }

        /// <summary>Создаёт контакт с указанными значениями. null заменяется на пустую строку.</summary>
        public Contact(string name, string phoneNumber, string email)
        {
            Name = name ?? string.Empty;
            PhoneNumber = phoneNumber ?? string.Empty;
            Email = email ?? string.Empty;
        }

        /// <summary>Создаёт глубокую копию текущего контакта.</summary>
        public Contact Clone() => new Contact(Name, PhoneNumber, Email);

        /// <summary>Событие изменения свойства.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Вызывает событие PropertyChanged для указанного свойства.</summary>
        protected void OnPropertyChanged([CallerMemberName] string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}