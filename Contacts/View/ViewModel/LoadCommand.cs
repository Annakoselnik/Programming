using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using View.Model.Services;

namespace View.ViewModel
{
    public class LoadCommand : ICommand
    {
        private readonly MainVM _mainVM;
        private readonly ContactSerializer _serializer;

        /// <summary>
        /// Конструктор команды загрузки
        /// </summary>
        /// <param name="mainVM">Экземпляр MainVM</param>
        /// <param name="serializer">Сериализатор для загрузки</param>
        public LoadCommand(MainVM mainVM, ContactSerializer serializer)
        {
            _mainVM = mainVM ?? throw new ArgumentNullException(nameof(mainVM));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <summary>
        /// Определяет, может ли команда быть выполнена
        /// </summary>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Выполняет команду загрузки
        /// </summary>
        public void Execute(object parameter)
        {
            var contact = _serializer.Load();

            _mainVM.Name = contact.Name;
            _mainVM.PhoneNumber = contact.PhoneNumber;
            _mainVM.Email = contact.Email;
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
