using Pixellation.Components.Dialogs.StringInputDialog;
using Pixellation.Components.Editor;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            RaiseLayerListPropertyInitialized += (a, b) => {
                LayerManager.VisualsChanged += UpdateLayerList;
                UpdateLayerList(a, EventArgs.Empty);
                // Initial layer selection
                SelectLayer();
            };
            InitializeComponent();
        }

        private void SelectLayer(int index = 0)
        {
            if (LayerManager != null && LayerManager.GetLayers().Count > 0 && layerList.Items.Count > 0)
            {
                LayerManager?.SetActiveLayer(index);
                layerList.SelectedIndex = index;
                layerList.SelectedItem = layerList.Items[index];
            }
        }

        private void UpdateLayerList(object sender, EventArgs e)
        {
            layerList.ItemsSource = LayerManager.GetLayers();
            layerList.Items.Refresh();
        }

        private void LayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectLayer(layerList.SelectedIndex);
        }

        private void AddLayer(object sender, RoutedEventArgs e)
        {
            var newLayerDialog = new StringInputDialog("New Layer", "Layername");
            if (newLayerDialog.ShowDialog() == true)
            {
                LayerManager?.AddLayer(newLayerDialog.Answer ?? (new DateTime()).Ticks.ToString());
                SelectLayer();
            }
        }

        private void DeleteLayer(object sender, RoutedEventArgs e)
        {
            LayerManager?.RemoveLayer(layerList.SelectedIndex);
        }

        private void MoveLayerUp(object sender, RoutedEventArgs e)
        {
            LayerManager?.MoveUp(layerList.SelectedIndex);
        }

        private void MoveLayerDown(object sender, RoutedEventArgs e)
        {
            LayerManager?.MoveDown(layerList.SelectedIndex);
        }
    }
}