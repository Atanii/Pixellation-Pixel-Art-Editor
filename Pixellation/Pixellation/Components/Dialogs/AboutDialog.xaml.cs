using System.Windows;

namespace Pixellation.Components.Dialogs.AboutDialog
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        /// <summary>
        /// Dialog containing information about Pixellation.
        /// </summary>
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }
    }
}