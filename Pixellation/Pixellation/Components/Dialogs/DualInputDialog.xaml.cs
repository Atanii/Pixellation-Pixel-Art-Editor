using Pixellation.Utils;
using System.Windows;

namespace Pixellation.Components.Dialogs
{
    /// <summary>
    /// Interaction logic for DualInputDialog.xaml
    /// </summary>
    public partial class DualInputDialog : Window
    {
        public string InputLabel1Text { get; private set; } = "Input 1";
        public string InputLabel2Text { get; private set; } = "Input 2";
        public string DialogTitle { get; private set; } = "Input Dialog";

        public string Answer
        {
            get { return input1.Text + ";" + input2.Text; }
        }

        public DualInputDialog(string title, string input1Name, string input2Name)
        {
            DialogTitle = title;
            InputLabel1Text = input1Name;
            InputLabel2Text = input2Name;

            InitializeComponent();

            input1.Focus();
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
            if (input1.Text != string.Empty && input2.Text != string.Empty)
            {
                this.DialogResult = true;
                return;
            }
            MBox.Error(Properties.Messages.ErrorEmptyInput);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}