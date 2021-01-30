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
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            l.LayerName = txtLayerName.Text;
            this.DialogResult = true;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public bool? ShowDialog(DrawingLayer layer) 
        {
            l = layer;
            txtLayerName.Text = l.LayerName;
            return ShowDialog();
        }
    }
}