using Pixellation.Utils;
using System.Windows;

namespace Pixellation.Components.Dialogs
{
    /// <summary>
    /// Interaction logic for StringTripleDialog.xaml
    /// </summary>
    public partial class StringTripleDialog : Window
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
        /// Label text for third input.
        /// </summary>
        public string InputLabel3Text { get; private set; } = "Input 3";

        /// <summary>
        /// Dialog title.
        /// </summary>
        public string DialogTitle { get; private set; } = "Input Dialog";

        /// <summary>
        /// Entered data.
        /// </summary>
        public string Answer
        {
            get { return txtString1.Text + ";" + txtString2.Text + ";" + txtString3.Text; }
        }

        /// <summary>
        /// Inits a dialog with three textfield.
        /// </summary>
        /// <param name="title"><see cref="DialogTitle"/>.</param>
        /// <param name="input1Name"><see cref="InputLabel1Text"/>.</param>
        /// <param name="input2Name"><see cref="InputLabel2Text"/>.</param>
        public StringTripleDialog(string title, string input1Name, string input2Name, string input3Name)
        {
            DialogTitle = title;

            InputLabel1Text = input1Name;
            InputLabel2Text = input2Name;
            InputLabel3Text = input3Name;

            InitializeComponent();

            txtString1.Focus();
        }

        /// <summary>
        /// Return input or show error if inputfield is empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok(object sender, RoutedEventArgs e)
        {
            if (txtString1.Text != string.Empty && txtString2.Text != string.Empty && txtString3.Text != string.Empty)
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

        /// <summary>
        /// <see cref="Ok(object, RoutedEventArgs)"/> on enter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Ok(sender, e);
            }
        }
    }
}