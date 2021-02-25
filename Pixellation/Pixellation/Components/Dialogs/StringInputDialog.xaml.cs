using System.Windows;

namespace Pixellation.Components.Dialogs.StringInputDialog
{
    /// <summary>
    /// Interaction logic for StringInputDialog.xaml
    /// </summary>
    public partial class StringInputDialog : Window
    {
        public string LabelText { get; private set; } = "Input";
        public string DialogTitle { get; private set; } = "Input Dialog";

        public string Answer
        {
            get { return txtInput.Text; }
        }

        public StringInputDialog(string title, string labelText)
        {
            DialogTitle = title;
            LabelText = labelText;

            InitializeComponent();

            txtInput.Focus();
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            if (Answer != "")
            {
                this.DialogResult = true;
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}