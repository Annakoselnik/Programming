using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace View.Controls
{
    public partial class ContactControl : UserControl
    {
        public ContactControl()
        {
            InitializeComponent();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9+\-\(\)\s\.]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                var regex = new Regex(@"^[0-9+\-\(\)\s\.]*$");
                if (!regex.IsMatch(text))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }
    }
}