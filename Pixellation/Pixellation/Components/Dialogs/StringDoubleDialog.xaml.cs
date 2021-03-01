using System;
using System.Collections.Generic;
using System.Windows;

namespace Pixellation.Components.Dialogs
{
    /// <summary>
    /// Interaction logic for StringDoubleDialog.xaml
    /// </summary>
    public partial class StringDoubleDialog : Window
    {
        /// <summary>
        /// Entered data. String as key, double as value.
        /// </summary>
        public KeyValuePair<string, double> Answer { get; private set; }

        /// <summary>
        /// Inputdialog for a string and a double input.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="input1Name">String input label text.</param>
        /// <param name="input2Name">Double input label text.</param>
        /// <param name="oldName">Previous string value.</param>
        /// <param name="oldDouble">Previous double value.</param>
        public StringDoubleDialog(string title, string input1Name, string input2Name, string oldName = "", double oldDouble = 0d)
        {
            InitializeComponent();

            Title = title;
            input1Label.Content = input1Name;
            input2Label.Content = input2Name;

            txtString.Focus();

            txtString.Text = oldName;
            txtDouble.Text = oldDouble.ToString();
        }

        /// <summary>
        /// Returns entered input. Shows error if no number was entered into the double inputfield.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save(object sender, RoutedEventArgs e)
        {
            var strTmp = txtDouble.Text.Replace('.', ',');
            if (double.TryParse(strTmp, out double newDouble))
            {
                newDouble = Math.Clamp(newDouble, 0.0, 1.0);

                Answer = new KeyValuePair<string, double>(txtString.Text, newDouble);

                this.DialogResult = true;
            } else
            {
                MessageBox.Show(string.Format(Properties.Messages.ErrorIsNotANumber, txtDouble.Text), "Error");
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

        /// <summary>
        /// <see cref="Save(object, RoutedEventArgs)"/> on enter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Save(sender, e);
            }
        }
    }
}