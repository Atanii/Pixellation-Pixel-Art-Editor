using Pixellation.Components.Dialogs;
using Pixellation.Components.Dialogs.StringInputDialog;
using Pixellation.Components.Editor;
using Pixellation.Components.Event;
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
        private static event EventHandler<DependencyPropertyChangedEventArgs> RaiseLayerListPropertyChanged;

        public ILayerManager LayerManager
        {
            get { return (ILayerManager)GetValue(LayerManagerProperty); }
            set { SetValue(LayerManagerProperty, value); }
        }

        public static readonly DependencyProperty LayerManagerProperty =
         DependencyProperty.Register("LayerManager", typeof(ILayerManager), typeof(LayerPalette), new FrameworkPropertyMetadata(
             default,
             (s, e) => { RaiseLayerListPropertyChanged?.Invoke(s, e); }
        ));

        public LayerPalette()
        {
            RaiseLayerListPropertyChanged += (a, b) =>
            {
                PixelEditor.LayerListChanged += UpdateLayerList;
                PixelEditor.FrameListChanged += UpdateLayerList;
                UpdateLayerList(a, PixelEditorLayerEventArgs.Empty);
                SelectLayer();
            };
            InitializeComponent();
        }

        private void SelectLayer(int index = 0)
        {
            if (LayerManager != null && LayerManager.Layers.Count > 0)
            {
                if (layerList.Items.Count == 0)
                {
                    layerList.ItemsSource = LayerManager.Layers;
                    layerList.Items.Refresh();
                }
                LayerManager?.SetActiveLayer(index);
                layerList.SelectedIndex = index;
                layerList.SelectedItem = layerList.Items[index];
            }
        }

        private void UpdateLayerList(object sender, PixelEditorLayerEventArgs e)
        {
            layerList.ItemsSource = LayerManager.Layers;
            layerList.Items.Refresh();

            if (e != PixelEditorLayerEventArgs.Empty && e.NewIndexOfActiveLayer != -1)
            {
                SelectLayer(e.NewIndexOfActiveLayer);
            }
        }

        private void UpdateLayerList(object sender, PixelEditorFrameEventArgs e)
        {
            layerList.ItemsSource = LayerManager.Layers;
            layerList.Items.Refresh();
            SelectLayer();
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
                    LayerManager.AddLayer(newLayerDialog.Answer);
                    LayerManager.SaveState(IPixelEditorEventType.ADDLAYER, layerList.Items.Count > 0 ? layerList.SelectedIndex : -1);
                }
                else
                {
                    LayerManager.SaveState(IPixelEditorEventType.ADDLAYER, layerList.Items.Count > 0 ? layerList.SelectedIndex : -1);
                    LayerManager.AddLayer(newLayerDialog.Answer);
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
            if (layerList.Items.Count > 1)
            {
                LayerManager.SaveState(IPixelEditorEventType.MOVELAYERDOWN, layerList.SelectedIndex);
                LayerManager.MoveLayerDown(layerList.SelectedIndex);
            }
        }

        private void MergeLayer(object sender, RoutedEventArgs e)
        {
            if (layerList.Items.Count > 1 && layerList.Items.Count > (layerList.SelectedIndex + 1))
            {
                LayerManager.SaveState(IPixelEditorEventType.MERGELAYER, layerList.SelectedIndex);
                LayerManager.MergeLayerDownward(layerList.SelectedIndex);
            }
        }

        private void ClearLayer(object sender, RoutedEventArgs e)
        {
            if (layerList.SelectedIndex < layerList.Items.Count)
            {
                LayerManager.SaveState(IPixelEditorEventType.LAYER_PIXELS_CHANGED, layerList.SelectedIndex);
                LayerManager.ClearLayer(layerList.SelectedIndex);
            }
        }

        private void OpenLayerSettingsDialog(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LayerManager != null)
            {
                var layer = LayerManager.ActiveLayer;
                var strDoubleDialog = new StringDoubleDialog("Layer Settings", "Name", "Opacity", layer.LayerName, layer.Opacity);
                if ((bool)strDoubleDialog.ShowDialog())
                {
                    // Due to lost focus (Note: "Focus();" doesn't help.)
                    SelectLayer(layerList.SelectedIndex);
                    
                    LayerManager.SaveState(IPixelEditorEventType.LAYER_INNER_PROPERTY_UPDATE, layerList.SelectedIndex);

                    layer.LayerName = strDoubleDialog.Answer.Key;
                    layer.Opacity = strDoubleDialog.Answer.Value;

                    layerList.Items.Refresh();
                }
            }
        }
    }
}