using Pixellation.Components.Editor;
using System;
using System.Windows;

namespace Pixellation.Components.Dialogs
{
    /// <summary>
    /// Interaction logic for LayerSettingsDialog.xaml
    /// </summary>
    public partial class LayerSettingsDialog : Window
    {
        private DrawingLayer l;

        public LayerSettingsDialog()
        {
            InitializeComponent();
            txtLayerOpacity.Focus();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            l.LayerName = txtLayerName.Text;
            var strTmp = txtLayerOpacity.Text.Replace('.', ',');
            if (double.TryParse(strTmp, out double newOpacity))
            {
                newOpacity = Math.Clamp(newOpacity, 0.0, 1.0);
                l.Opacity = newOpacity;
                this.DialogResult = true;
            } else
            {
                MessageBox.Show($"Value {txtLayerOpacity.Text} is not a number!", "Error");
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public bool? ShowDialog(DrawingLayer layer) 
        {
            l = layer;
            txtLayerName.Text = l.LayerName;
            txtLayerOpacity.Text = l.Opacity.ToString();
            return ShowDialog();
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