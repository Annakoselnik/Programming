using System.Windows;
using System.Windows.Controls;

namespace Contacts.View.Controls
{
    public partial class ContactControl : UserControl
    {
        public ContactControl()
        {
            InitializeComponent();
        }

        private void TextBox_ValidationError(object sender, ValidationErrorEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (e.Action == ValidationErrorEventAction.Added)
            {
                if (e.Error.ErrorContent != null)
                    textBox.ToolTip = e.Error.ErrorContent.ToString();
            }
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                textBox.ToolTip = null;
            }
        }
    }
}