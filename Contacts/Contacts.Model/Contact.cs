using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Contacts.Model
{
    public class Contact : ObservableObject, INotifyDataErrorInfo
    {
        private string _name;
        private string _phoneNumber;
        private string _email;
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value ?? string.Empty))
                    Validate(nameof(Name), _name);
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (SetProperty(ref _phoneNumber, value ?? string.Empty))
                    Validate(nameof(PhoneNumber), _phoneNumber);
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value ?? string.Empty))
                    Validate(nameof(Email), _email);
            }
        }

        public Contact() => Name = PhoneNumber = Email = string.Empty;
        public Contact(string name, string phone, string email)
        {
            Name = name ?? string.Empty;
            PhoneNumber = phone ?? string.Empty;
            Email = email ?? string.Empty;
        }

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
        public IEnumerable GetErrors(string propertyName) =>
            _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
    }
}