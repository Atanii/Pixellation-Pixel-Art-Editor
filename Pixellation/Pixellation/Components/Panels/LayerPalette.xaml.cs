using Pixellation.Components.Dialogs;
using Pixellation.Components.Dialogs.StringInputDialog;
using Pixellation.Components.Editor;
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
                LayerManager.LayerListChanged += UpdateLayerList;
                UpdateLayerList(a, PixelEditorEventArgs.Empty);
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

        private void UpdateLayerList(object sender, PixelEditorEventArgs e)
        {
            layerList.ItemsSource = LayerManager.GetLayers();
            layerList.Items.Refresh();
            if (e != PixelEditorEventArgs.Empty && e.NewIndexOfActiveLayer != -1)
            {
                SelectLayer(e.NewIndexOfActiveLayer);
            }
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
                if (layerList.Items.Count == 0)
                {
                    LayerManager.AddLayer(newLayerDialog.Answer ?? (new DateTime()).Ticks.ToString());
                    LayerManager.SaveState(IPixelEditorEventType.ADDLAYER, layerList.Items.Count > 0 ? layerList.SelectedIndex : -1);
                }
                else
                {
                    LayerManager.SaveState(IPixelEditorEventType.ADDLAYER, layerList.Items.Count > 0 ? layerList.SelectedIndex : -1);
                    LayerManager.AddLayer(newLayerDialog.Answer ?? (new DateTime()).Ticks.ToString());
                }
            }
        }

        private void DeleteLayer(object sender, RoutedEventArgs e)
        {
            if (layerList.Items.Count > 0)
            {
                LayerManager.SaveState(IPixelEditorEventType.REMOVELAYER, layerList.SelectedIndex);
                LayerManager.RemoveLayer(layerList.SelectedIndex);
            }
        }

        private void DuplicateLayer(object sender, RoutedEventArgs e)
        {
            if (layerList.Items.Count > 0)
            {
                LayerManager.SaveState(IPixelEditorEventType.DUPLICATELAYER, layerList.SelectedIndex);
                LayerManager.DuplicateLayer(layerList.SelectedIndex);
            }
        }

        private void MoveLayerUp(object sender, RoutedEventArgs e)
        {
            if (layerList.SelectedIndex > 0)
            {
                LayerManager.SaveState(IPixelEditorEventType.MOVELAYERUP, layerList.SelectedIndex);
                LayerManager.MoveLayerUp(layerList.SelectedIndex);
            }
        }

        private void MoveLayerDown(object sender, RoutedEventArgs e)
        {
            if (layerList.SelectedIndex < (layerList.Items.Count - 1))
            {
                LayerManager.SaveState(IPixelEditorEventType.MOVELAYERDOWN, layerList.SelectedIndex);
                LayerManager.MoveLayerDown(layerList.SelectedIndex);
            }
        }

        private void MergeLayer(object sender, RoutedEventArgs e)
        {
            if (layerList.SelectedIndex < (layerList.Items.Count - 1))
            {
                LayerManager.SaveState(IPixelEditorEventType.MERGELAYER, layerList.SelectedIndex);
                LayerManager.MergeLayerDownward(layerList.SelectedIndex);
            }
        }

        private void OpenLayerSettingsDialog(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LayerManager != null)
            {
                var newImgDialog = new LayerSettingsDialog();
                newImgDialog.ShowDialog(LayerManager.GetLayer(layerList.SelectedIndex));
                // Due to lost focus (Note: "Focus();" doesn't help.)
                SelectLayer(layerList.SelectedIndex);
            }
        }
    }
}