using System.Windows;

namespace Pixellation.Components.Dialogs.NewImageDialog
{
    /// <summary>
    /// Interaction logic for NewImageDialog.xaml
    /// </summary>
    public partial class NewImageDialog : Window
    {
        public string Answer
        {
            get { return txtWidth.Text + ";" + txtHeight.Text; }
        }

        public NewImageDialog()
        {
            InitializeComponent();
            txtWidth.Focus();
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}