using System.Windows;

namespace Pixellation.Components.Dialogs.StringInputDialog
{
    /// <summary>
    /// Interaction logic for StringInputDialog.xaml
    /// </summary>
    public partial class StringInputDialog : Window
    {
        /// <summary>
        /// Text for first inputlabel.
        /// </summary>
        public string LabelText { get; private set; } = "Input";

        /// <summary>
        /// Title of dialog.
        /// </summary>
        public string DialogTitle { get; private set; } = "Input Dialog";

        /// <summary>
        /// Entered data.
        /// </summary>
        public string Answer
        {
            get { return txtInput.Text; }
        }

        /// <summary>
        /// Dialog for a single string input.
        /// </summary>
        /// <param name="title"><see cref="DialogTitle"/>.</param>
        /// <param name="labelText"><see cref="LabelText"/>.</param>
        public StringInputDialog(string title, string labelText)
        {
            DialogTitle = title;
            LabelText = labelText;

            InitializeComponent();

            txtInput.Focus();
        }

        /// <summary>
        /// Returns entered input or shows error if inputfield is empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Create(object sender, RoutedEventArgs e)
        {
            if (Answer != "")
            {
                this.DialogResult = true;
            }
        }

        /// <summary>
        /// Cancel input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}