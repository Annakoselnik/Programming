using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace View.Model
{
    /// <summary>Модель контакта с поддержкой уведомлений об изменении свойств и валидацией.</summary>
    public class Contact : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string _name;
        private string _phoneNumber;
        private string _email;
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

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
                    Validate(nameof(Name), _name);
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
                    Validate(nameof(PhoneNumber), _phoneNumber);
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
                    Validate(nameof(Email), _email);
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

        private void Validate(string propertyName, string value)
        {
            _errors.Remove(propertyName);
            var errors = new List<string>();

            switch (propertyName)
            {
                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(value))
                        errors.Add("Имя не может быть пустым.");
                    else if (value.Length > 100)
                        errors.Add("Имя не должно превышать 100 символов.");
                    break;

                case nameof(PhoneNumber):
                    if (string.IsNullOrWhiteSpace(value))
                        errors.Add("Номер телефона не может быть пустым.");
                    else if (value.Length > 100)
                        errors.Add("Номер телефона не должен превышать 100 символов.");
                    else
                    {
                        string pattern = @"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$";
                        if (!Regex.IsMatch(value, pattern))
                            errors.Add("Номер телефона должен быть в формате: +7 (999) 111-22-33");
                    }
                    break;

                case nameof(Email):
                    if (string.IsNullOrWhiteSpace(value))
                        errors.Add("Email не может быть пустым.");
                    else if (value.Length > 100)
                        errors.Add("Email не должен превышать 100 символов.");
                    else
                    {
                        if (!value.Contains('@'))
                            errors.Add("Email должен содержать символ @.");
                        else if (value.IndexOf('@') == 0 || value.IndexOf('@') == value.Length - 1)
                            errors.Add("Email не может начинаться или заканчиваться символом @.");
                        else
                        {
                            string domain = value.Substring(value.IndexOf('@') + 1);
                            if (string.IsNullOrEmpty(domain) || !domain.Contains('.'))
                                errors.Add("Домен email должен содержать точку (например, mail.com).");
                            else if (domain.IndexOf('.') == 0 || domain.LastIndexOf('.') == domain.Length - 1)
                                errors.Add("Домен email не может начинаться или заканчиваться точкой.");
                        }
                    }
                    break;
            }

            if (errors.Any())
                _errors[propertyName] = errors;

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return null;
            return _errors[propertyName];
        }

        protected void OnPropertyChanged([CallerMemberName] string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}