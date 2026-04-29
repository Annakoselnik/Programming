using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.Model.Services
{
    /// <summary>Сериализация списка контактов в JSON-файл и обратно.</summary>
    public class ContactSerializer
    {
        private string _filePath;

        /// <summary>Путь к JSON-файлу. Если null, используется путь по умолчанию.</summary>
        public string FilePath
        {
            get => _filePath;
            set => _filePath = value ?? GetDefaultPath();
        }

        /// <summary>Создаёт сериализатор с указанным путём (или путём по умолчанию).</summary>
        public ContactSerializer(string filePath = null)
        {
            FilePath = filePath ?? GetDefaultPath();
        }

        private string GetDefaultPath()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string contactsFolder = Path.Combine(documentsPath, "Contacts");
            if (!Directory.Exists(contactsFolder))
                Directory.CreateDirectory(contactsFolder);
            return Path.Combine(contactsFolder, "contacts.json");
        }

        /// <summary>Сохраняет список контактов в JSON-файл.</summary>
        public void Save(IEnumerable<Contact> contacts)
        {
            if (contacts == null) throw new ArgumentNullException(nameof(contacts));
            string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        /// <summary>Загружает список контактов из JSON-файла. Если файл отсутствует, возвращает пустой список.</summary>
        public List<Contact> Load()
        {
            if (!File.Exists(FilePath))
                return new List<Contact>();
            string json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<Contact>>(json) ?? new List<Contact>();
        }
    }
}