using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace View.Model
{
    /// <summary>
    /// Класс, представляющий контакт с именем, телефоном и email
    /// </summary>
    public class Contact 
    {
        private string _name;
        private string _phoneNumber;
        private string _email;

        /// <summary>
        /// Имя контакта
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value ?? string.Empty;
        }

        /// <summary>
        /// Номер телефона контакта
        /// </summary>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value ?? string.Empty;
        }

        /// <summary>
        /// Email контакта
        /// </summary>
        public string Email
        {
            get => _email;
            set => _email = value ?? string.Empty;
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Contact()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
            Email = string.Empty;
        }

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="name">Имя контакта</param>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <param name="email">Email контакта</param>
        public Contact(string name, string phoneNumber, string email)
        {
            Name = name ?? string.Empty;
            PhoneNumber = phoneNumber ?? string.Empty;
            Email = email ?? string.Empty;
        }
    }
}