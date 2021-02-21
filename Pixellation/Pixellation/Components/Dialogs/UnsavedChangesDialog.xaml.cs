using System.Windows;

namespace Pixellation.Components.Dialogs
{
    /// <summary>
    /// Interaction logic for UnsavedChangesDialog.xaml
    /// </summary>
    public partial class UnsavedChangesDialog : Window
    {
        public UnsavedChangesDialog(string title)
        {
            InitializeComponent();
            Title = title;
        }

        public enum CloseDialogAnswer
        {
            SAVE, DONT_SAVE, CANCEL
        }

        public CloseDialogAnswer Answer { get; private set; }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Answer = CloseDialogAnswer.SAVE;
            this.DialogResult = true;
        }

        private void BtnDontSave_Click(object sender, RoutedEventArgs e)
        {
            Answer = CloseDialogAnswer.DONT_SAVE;
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Answer = CloseDialogAnswer.CANCEL;
            this.DialogResult = false;
        }
    }
}