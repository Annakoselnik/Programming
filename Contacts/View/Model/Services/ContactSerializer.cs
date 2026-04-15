using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.Model.Services
{
    /// <summary>
    /// Класс для сериализации и десериализации контакта в JSON
    /// </summary>
    public class ContactSerializer
    {
        private string _filePath;

        /// <summary>
        /// Путь к файлу для сохранения/загрузки контакта
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set => _filePath = value ?? GetDefaultPath();
        }

        /// <summary>
        /// Конструктор с указанием пути к файлу
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        public ContactSerializer(string filePath = null)
        {
            FilePath = filePath ?? GetDefaultPath();
        }

        /// <summary>
        /// Получает путь по умолчанию (Мои документы/Contacts/contacts.json)
        /// </summary>
        /// <returns>Путь по умолчанию</returns>
        private string GetDefaultPath()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string contactsFolder = Path.Combine(documentsPath, "Contacts");

            // Создаем папку, если её нет
            if (!Directory.Exists(contactsFolder))
            {
                Directory.CreateDirectory(contactsFolder);
            }

            return Path.Combine(contactsFolder, "contacts.json");
        }

        /// <summary>
        /// Сохраняет контакт в файл
        /// </summary>
        /// <param name="contact">Контакт для сохранения</param>
        public void Save(Contact contact)
        {
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            string json = JsonConvert.SerializeObject(contact, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        /// <summary>
        /// Загружает контакт из файла
        /// </summary>
        /// <returns>Загруженный контакт или новый контакт, если файл не найден</returns>
        public Contact Load()
        {
            if (!File.Exists(FilePath))
            {
                return new Contact();
            }

            string json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Contact>(json) ?? new Contact();
        }
    }
}
