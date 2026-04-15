using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using View.Model.Services;


namespace View.ViewModel
{
    public class SaveCommand : ICommand
    {
        private readonly MainVM _mainVM;
        private readonly ContactSerializer _serializer;

        /// <summary>
        /// Конструктор команды сохранения
        /// </summary>
        /// <param name="mainVM">Экземпляр MainVM</param>
        /// <param name="serializer">Сериализатор для сохранения</param>
        public SaveCommand(MainVM mainVM, ContactSerializer serializer)
        {
            _mainVM = mainVM ?? throw new ArgumentNullException(nameof(mainVM));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <summary>
        /// Определяет, может ли команда быть выполнена
        /// </summary>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Выполняет команду сохранения
        /// </summary>
        public void Execute(object parameter)
        {
            var contact = new Model.Contact
            {
                Name = _mainVM.Name,
                PhoneNumber = _mainVM.PhoneNumber,
                Email = _mainVM.Email
            };

            _serializer.Save(contact);
        }

        /// <summary>
        /// Событие изменения возможности выполнения команды
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
