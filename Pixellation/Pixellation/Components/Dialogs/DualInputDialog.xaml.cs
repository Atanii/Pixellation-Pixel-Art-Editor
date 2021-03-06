using Pixellation.Utils;
using System.Windows;

namespace Pixellation.Components.Dialogs
{
    /// <summary>
    /// Interaction logic for DualInputDialog.xaml
    /// </summary>
    public partial class DualInputDialog : Window
    {
        /// <summary>
        /// Label text for first input.
        /// </summary>
        public string InputLabel1Text { get; private set; } = "Input 1";

        /// <summary>
        /// Label text for second input.
        /// </summary>
        public string InputLabel2Text { get; private set; } = "Input 2";

        /// <summary>
        /// Dialog title.
        /// </summary>
        public string DialogTitle { get; private set; } = "Input Dialog";

        /// <summary>
        /// Entered data.
        /// </summary>
        public string Answer
        {
            get { return input1.Text + ";" + input2.Text; }
        }

        /// <summary>
        /// Inits a dialog with two textfield.
        /// </summary>
        /// <param name="title"><see cref="DialogTitle"/>.</param>
        /// <param name="input1Name"><see cref="InputLabel1Text"/>.</param>
        /// <param name="input2Name"><see cref="InputLabel2Text"/>.</param>
        public DualInputDialog(string title, string input1Name, string input2Name)
        {
            DialogTitle = title;
            InputLabel1Text = input1Name;
            InputLabel2Text = input2Name;

            InitializeComponent();

            input1.Focus();
        }

        /// <summary>
        /// Return input or show error if inputfield is empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok(object sender, RoutedEventArgs e)
        {
            if (input1.Text != string.Empty && input2.Text != string.Empty)
            {
                this.DialogResult = true;
                return;
            }
            MBox.Error(Properties.Messages.ErrorEmptyInput);
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