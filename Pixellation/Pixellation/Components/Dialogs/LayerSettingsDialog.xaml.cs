using System;
using System.Collections.Generic;
using System.Windows;

namespace Pixellation.Components.Dialogs
{
    /// <summary>
    /// Interaction logic for LayerSettingsDialog.xaml
    /// </summary>
    public partial class LayerSettingsDialog : Window
    {
        public KeyValuePair<string, double> Answer { get; private set; }

        public LayerSettingsDialog(string oldName, double oldOpacity)
        {
            InitializeComponent();

            txtLayerName.Focus();

            txtLayerName.Text = oldName;
            txtLayerOpacity.Text = oldOpacity.ToString();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var strTmp = txtLayerOpacity.Text.Replace('.', ',');
            if (double.TryParse(strTmp, out double newOpacity))
            {
                newOpacity = Math.Clamp(newOpacity, 0.0, 1.0);

                Answer = new KeyValuePair<string, double>(txtLayerName.Text, newOpacity);

                this.DialogResult = true;
            } else
            {
                MessageBox.Show(string.Format(Properties.Messages.ErrorIsNotANumber, txtLayerOpacity.Text), "Error");
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