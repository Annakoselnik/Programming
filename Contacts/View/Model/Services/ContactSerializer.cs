using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.Model.Services
{
    public class ContactSerializer
    {
        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value ?? GetDefaultPath();
        }

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

        public void Save(IEnumerable<Contact> contacts)
        {
            if (contacts == null) throw new ArgumentNullException(nameof(contacts));
            string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public List<Contact> Load()
        {
            if (!File.Exists(FilePath))
                return new List<Contact>();
            string json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<Contact>>(json) ?? new List<Contact>();
        }
    }
}