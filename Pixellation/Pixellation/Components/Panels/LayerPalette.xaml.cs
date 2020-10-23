using Pixellation.Components.Dialogs.StringInputDialog;
using Pixellation.Components.Editor;
using System;
using System.Windows;
using System.Windows.Controls;
using static Pixellation.Components.Editor.PixelEditor;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for Layers.xaml
    /// </summary>
    public partial class LayerPalette : UserControl
    {
        private static event EventHandler<DependencyPropertyChangedEventArgs> RaiseLayerListPropertyInitialized;

        public IVisualManager LayerManager
        {
            get { return (IVisualManager)GetValue(LayerListProperty); }
            set { SetValue(LayerListProperty, value); }
        }

        public static readonly DependencyProperty LayerListProperty =
         DependencyProperty.Register("LayerManager", typeof(IVisualManager), typeof(LayerPalette), new FrameworkPropertyMetadata(
             default,
             (s, e) => { RaiseLayerListPropertyInitialized?.Invoke(s, e); }
        ));

        public LayerPalette()
        {
            RaiseLayerListPropertyInitialized += (a, b) => { LayerManager.VisualsChanged += UpdateLayerList; UpdateLayerList(a, EventArgs.Empty); };
            InitializeComponent();
        }

        private void UpdateLayerList(object sender, EventArgs e)
        {
            layerList.ItemsSource = LayerManager.GetLayers();
            layerList.Items.Refresh();
        }

        private void LayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LayerManager?.SetActiveLayer(layerList.SelectedIndex);
        }

        private void AddLayer(object sender, RoutedEventArgs e)
        {
            var newLayerDialog = new StringInputDialog("New Layer", "Layername");
            if (newLayerDialog.ShowDialog() == true)
                LayerManager?.AddLayer(newLayerDialog.Answer ?? (new DateTime()).Ticks.ToString());
        }

        private void DeleteLayer(object sender, RoutedEventArgs e)
        {
            LayerManager?.RemoveLayer(layerList.SelectedIndex);
        }
    }
}