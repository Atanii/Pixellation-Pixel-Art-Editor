using System.Windows;

namespace Pixellation.Components.Dialogs
{
    /// <summary>
    /// Interaction logic for NewProjectDialog.xaml
    /// </summary>
    public partial class NewProjectDialog : Window
    {
        public string Answer
        {
            get { return txtWidth.Text + ";" + txtHeight.Text; }
        }

        public NewProjectDialog()
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