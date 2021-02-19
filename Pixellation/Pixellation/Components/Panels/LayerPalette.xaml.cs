using Pixellation.Components.Dialogs;
using Pixellation.Components.Dialogs.StringInputDialog;
using Pixellation.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

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
            RaiseLayerListPropertyInitialized += (a, b) =>
            {
                LayerManager.RaiseImageUpdatedEvent += UpdateLayerList;
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
                int index = LayerManager.AddLayer(newLayerDialog.Answer ?? (new DateTime()).Ticks.ToString());
                SelectLayer(index);
            }
        }

        private void DeleteLayer(object sender, RoutedEventArgs e)
        {
            int index = LayerManager.RemoveLayer(layerList.SelectedIndex);
            if (index != -1)
            {
                SelectLayer(index);
            }
        }

        private void DuplicateLayer(object sender, RoutedEventArgs e)
        {
            int index = LayerManager.DuplicateLayer(layerList.SelectedIndex);
            SelectLayer(index);
        }

        private void MoveLayerUp(object sender, RoutedEventArgs e)
        {
            int index = LayerManager.MoveLayerUp(layerList.SelectedIndex);
            SelectLayer(index);
        }

        private void MoveLayerDown(object sender, RoutedEventArgs e)
        {
            int index = LayerManager.MoveLayerDown(layerList.SelectedIndex);
            SelectLayer(index);
        }

        private void OpenLayerSettingsDialog(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LayerManager != null)
            {
                var newImgDialog = new LayerSettingsDialog();
                newImgDialog.ShowDialog(LayerManager.GetLayer(layerList.SelectedIndex));
                layerList.Items.Refresh();
            }
        }
    }
}