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
        public KeyValuePair<string, double> Answer { get; private set; }

        public StringDoubleDialog(string title, string input1Name, string input2Name, string oldName, double oldDouble)
        {
            InitializeComponent();

            Title = title;
            input1Label.Content = input1Name;
            input2Label.Content = input2Name;

            txtString.Focus();

            txtString.Text = oldName;
            txtDouble.Text = oldDouble.ToString();
        }

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

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Save(sender, e);
            }
        }
    }
}