using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace View.Model
{
    public class Contact : INotifyPropertyChanged
    {
        private string _name;
        private string _phoneNumber;
        private string _email;

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

        public Contact()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
            Email = string.Empty;
        }

        public Contact(string name, string phoneNumber, string email)
        {
            Name = name ?? string.Empty;
            PhoneNumber = phoneNumber ?? string.Empty;
            Email = email ?? string.Empty;
        }

        public Contact Clone() => new Contact(Name, PhoneNumber, Email);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}