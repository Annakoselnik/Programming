using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Contacts.Model.Services
{
    public class ContactSerializer
    {
        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => _filePath = value ?? GetDefaultPath();
        }

        public ContactSerializer(string filePath = null) => FilePath = filePath ?? GetDefaultPath();

        private string GetDefaultPath()
        {
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folder = Path.Combine(docs, "Contacts");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            return Path.Combine(folder, "contacts.json");
        }

        public void Save(IEnumerable<Contact> contacts)
        {
            string json = JsonConvert.SerializeObject(contacts, Newtonsoft.Json.Formatting.Indented); File.WriteAllText(FilePath, json);
        }

        public List<Contact> Load()
        {
            if (!File.Exists(FilePath)) return new List<Contact>();
            string json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<Contact>>(json) ?? new List<Contact>();
        }
    }
}