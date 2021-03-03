using Pixellation.Components.Event;
using Pixellation.Interfaces;
using Pixellation.Properties;
using Pixellation.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor : ILayerManager
    {
        /// <summary>
        /// Event indicating change in <see cref="Layers"/>.
        /// </summary>
        public static event PixelEditorLayerEventHandler LayerListChanged;

        /// <summary>
        /// Currently selected layer.
        /// </summary>
        public DrawingLayer ActiveLayer { get; private set; }

        /// <summary>
        /// Index of the selected layer.
        /// </summary>
        public int ActiveLayerIndex => Layers.FindIndex(x => x.Id == ActiveLayer.Id);

        /// <summary>
        /// Currently edited layers.
        /// </summary>
        public List<DrawingLayer> Layers
        {
            get
            {
                if (Frames.Count >= (ActiveFrameIndex + 1))
                {
                    return Frames[ActiveFrameIndex].Layers;
                }
                else
                {
                    return new List<DrawingLayer>();
                }
            }
        }

        /// <summary>
        /// Calls Measure for layers.
        /// </summary>
        /// <param name="s"></param>
        public void MeasureAllLayer(System.Windows.Size s)
        {
            foreach (var l in Layers)
            {
                l?.Measure(s);
            }
            if (_onionLayer != null)
            {
                _onionLayer.Measure(s);
            }
            if (_drawPreview != null)
            {
                _drawPreview.Measure(s);
            }
        }

        /// <summary>
        /// Calls arrange for layers.
        /// </summary>
        /// <param name="s"></param>
        public void ArrangeAllLayer(System.Windows.Rect s)
        {
            foreach (var l in Layers)
            {
                l?.Arrange(s);
            }
            if (_onionLayer != null)
            {
                _onionLayer.Arrange(s);
            }
            if (_drawPreview != null)
            {
                _drawPreview.Arrange(s);
            }
        }

        /// <summary>
        /// Removes all child <see cref="Visual"/> from <see cref="PixelEditor"/>.
        /// </summary>
        public void DeleteAllVisualChildren()
        {
            foreach (var l in Layers)
            {
                RemoveVisualChild(l);
            }
            Layers.Clear();

            if (_onionLayer != null)
            {
                RemoveVisualChild(_onionLayer);
                _onionLayer = null;
            }
            if (_drawPreview != null)
            {
                RemoveVisualChild(_drawPreview);
                _drawPreview = null;
            }

            if (_borderLine != null)
            {
                RemoveVisualChild(_borderLine);
                _borderLine = null;
            }
            if (_gridLines != null)
            {
                RemoveVisualChild(_gridLines);
                _gridLines = null;
            }

            LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.CLEAR, -1));

            Frames.Clear();

            FrameListChanged?.Invoke(new PixelEditorFrameEventArgs(IPixelEditorEventType.CLEAR, -1));
        }

        /// <summary>
        /// Reset layers and helpervisuals on the visualtree by readding them.
        /// </summary>
        /// <param name="includeHelperVisuals">Include and recreate helpervisuals in the process.</param>
        public void ResetLayerAndHelperVisuals(bool includeHelperVisuals = true)
        {
            // Layers
            RemoveLayersFromVisualChildren();
            AddLayersToVisualChildren();

            // Onionlayer
            ResetOnionLayer();

            // Previewlayer
            ResetPreviewLayer();

            // Helpervisuals
            if (includeHelperVisuals)
            {
                ResetHelperVisuals();
            }
        }

        /// <summary>
        /// Remove all layers of the current frame from the visualtree.
        /// </summary>
        private void RemoveLayersFromVisualChildren()
        {
            foreach (var l in Layers)
            {
                RemoveVisualChild(l);
            }
        }

        /// <summary>
        /// Add all layers of the current frame to the visualtree.
        /// </summary>
        private void AddLayersToVisualChildren()
        {
            foreach (var l in Layers)
            {
                AddVisualChild(l);
            }
        }

        /// <summary>
        /// Refreshes layer used for drawpreview.
        /// </summary>
        public void ResetPreviewLayer()
        {
            if (_drawPreview != null)
            {
                RemoveVisualChild(_drawPreview);
                AddVisualChild(_drawPreview);
            }
        }

        /// <summary>
        /// Refreshes layer used for onion mode.
        /// </summary>
        public void ResetOnionLayer()
        {
            if (_onionLayer != null)
            {
                RemoveVisualChild(_onionLayer);
                AddVisualChild(_onionLayer);
                RefreshOnionLayer();
            }
        }

        /// <summary>
        /// (Re)initializes previewlayer.
        /// </summary>
        private void InitPreviewLayer()
        {
            _drawPreview = new DrawingLayer(this, "DrawPreview");
        }

        /// <summary>
        /// (Re)initializes onionlayer.
        /// </summary>
        private void InitOnionLayer()
        {
            OnionLayer = new DrawingLayer(this, "Onion", true, 0.5f);
        }

        /// <summary>
        /// Reset helpervisuals.
        /// </summary>
        public void ResetHelperVisuals()
        {
            if (_borderLine != null)
            {
                RemoveVisualChild(_borderLine);
                _borderLine = null;
            }

            if (_gridLines != null)
            {
                RemoveVisualChild(_gridLines);
                _gridLines = null;
            }

            if (ShowBorder)
            {
                _borderLine = CreateBorderLines();
                AddVisualChild(_borderLine);
            }

            if (ShowGrid)
            {
                _gridLines = CreateGridLines();
                AddVisualChild(_gridLines);
            }
        }

        /// <summary>
        /// Sets selected layer.
        /// </summary>
        /// <param name="layerIndex">Index of layers to be selected.</param>
        public void SetActiveLayer(int layerIndex = 0)
        {
            if (Layers.ElementAtOrDefault(layerIndex) != null)
            {
                ActiveLayer = Layers[layerIndex];
                RefreshVisualsThenSignalUpdate();
            }
        }

        /// <summary>
        /// Gets index for a layer.
        /// </summary>
        /// <param name="layer">Layer the index is asked for.</param>
        /// <returns>Index of the layer.</returns>
        public int GetIndex(DrawingLayer layer)
        {
            return Layers.FindIndex(x => x.Id == layer.Id);
        }

        #region Transform

        /// <summary>
        /// Mirrors drawn image.
        /// </summary>
        /// <param name="horizontally">Mirror horizontally or vertically.</param>
        /// <param name="allLayers">Only selected or all layers in the frame.</param>
        public void Mirror(bool horizontally, bool allLayers)
        {
            if (!allLayers && ActiveLayer != null)
            {
                ActiveLayer.Mirror(horizontally);
            }
            else
            {
                foreach (var l in Layers)
                {
                    l.Mirror(horizontally);
                }
            }
            RefreshVisualsThenSignalUpdate();
        }

        /// <summary>
        /// Rotates the image, applies to all image.
        /// </summary>
        /// <param name="counterClockWise">Counter- or clovkwise rotation.</param>
        public void Rotate(bool counterClockWise)
        {
            foreach (var l in Layers)
            {
                l.Rotate(counterClockWise ? 270 : 90);
            }
            _drawPreview.Rotate(counterClockWise ? 270 : 90);
            _onionLayer.Rotate(counterClockWise ? 270 : 90);

            var tmp = PixelHeight;
            PixelHeight = PixelWidth;
            PixelWidth = tmp;

            RefreshVisualsThenSignalUpdate();
        }

        /// <summary>
        /// Resizes edited image.
        /// </summary>
        /// <param name="newWidth">New width in pixels.</param>
        /// <param name="newHeight">New height in pixels.</param>
        public void Resize(int newWidth, int newHeight)
        {
            foreach (var l in Layers)
            {
                l.Resize(newWidth, newHeight);
            }
            _drawPreview.Resize(newWidth, newHeight);
            _onionLayer.Resize(newWidth, newHeight);

            PixelWidth = newWidth;
            PixelHeight = newHeight;
            Magnification = Settings.Default.DefaultMagnification;
        }

        #endregion Transform

        #region Add, Delete, Duplicate, Move Up, Move Down, Merge, Clear

        /// <summary>
        /// Adds a new layer.
        /// </summary>
        /// <param name="layer">Layer to be added.</param>
        /// <param name="layerIndex">Index of the new layer.</param>
        public void AddLayer(DrawingLayer layer, int layerIndex = 0)
        {
            Layers.Insert(layerIndex, layer);

            RefreshVisualsThenSignalUpdate();

            LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.ADDLAYER, layerIndex));
        }

        /// <summary>
        /// Adds a layer in the process of undoing or redoing an operation.
        /// </summary>
        /// <param name="layer">Layer to add.</param>
        /// <param name="layerIndex">Index to insert in.</param>
        public void AddLayerByUndoRedo(DrawingLayer layer, int layerIndex = 0)
        {
            Layers.Insert(layerIndex, layer);
            ActiveLayer = Layers[ActiveLayerIndex < 1 ? 0 : ActiveLayerIndex - 1];
            LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.ADDLAYER, ActiveLayerIndex));
            RefreshVisualsThenSignalUpdate();
        }

        /// <summary>
        /// Removes layer in the process of undoing or redoing an operation.
        /// </summary>
        /// <param name="layerIndex">Index of layer to remove.</param>
        public void RemoveLayerByUndoRedo(int layerIndex)
        {
            RemoveVisualChild(Layers[layerIndex]);
            Layers.RemoveAt(layerIndex);
            ActiveLayer = Layers[ActiveLayerIndex < 1 ? 0 : ActiveLayerIndex - 1];
            LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.REMOVELAYER, ActiveLayerIndex));
            RefreshVisualsThenSignalUpdate();
        }

        /// <summary>
        /// Adds a new layer.
        /// </summary>
        /// <param name="name">Name of the new layer.</param>
        /// <param name="layerIndex">Index of the new layer.</param>
        public void AddLayer(string name, int layerIndex = 0)
        {
            Layers.Insert(layerIndex, new DrawingLayer(this, name));

            RefreshVisualsThenSignalUpdate();

            LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.ADDLAYER, layerIndex));
        }

        /// <summary>
        /// Duplicates a layer.
        /// </summary>
        /// <param name="layerIndex">Index of the layer to duplicate.</param>
        public void DuplicateLayer(int layerIndex = 0)
        {
            Layers.Insert(layerIndex, Layers[layerIndex].Clone());

            RefreshVisualsThenSignalUpdate();

            LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.DUPLICATELAYER, layerIndex));
        }

        /// <summary>
        /// Moves a layer up (closer).
        /// </summary>
        /// <param name="layerIndex">Index of layer to move.</param>
        public void MoveLayerUp(int layerIndex)
        {
            var newLayerIndex = layerIndex;
            if (Layers.Count > layerIndex && layerIndex > 0)
            {
                var tmp = Layers[layerIndex];
                Layers.RemoveAt(layerIndex);
                --newLayerIndex;
                Layers.Insert(newLayerIndex, tmp);
                RefreshVisualsThenSignalUpdate();
                LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.MOVELAYERUP, newLayerIndex));
            }
        }

        /// <summary>
        /// Moves a layer down (behind).
        /// </summary>
        /// <param name="layerIndex">Index of layer to move.</param>
        public void MoveLayerDown(int layerIndex)
        {
            var newLayerIndex = layerIndex;
            if (layerIndex < Layers.Count() - 1)
            {
                var tmp = Layers[layerIndex];
                Layers.RemoveAt(layerIndex);
                ++newLayerIndex;
                Layers.Insert(newLayerIndex, tmp);
                RefreshVisualsThenSignalUpdate();
            }
            LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.MOVELAYERDOWN, newLayerIndex));
        }

        /// <summary>
        /// Removes layer.
        /// </summary>
        /// <param name="layer">Layer to remove.</param>
        private void RemoveLayer(DrawingLayer layer)
        {
            RemoveVisualChild(layer);
            Layers.Remove(layer);
        }

        /// <summary>
        /// Removes a layer.
        /// </summary>
        /// <param name="layerIndex">Index of the layer to remove.</param>
        public void RemoveLayer(int layerIndex)
        {
            if ((Layers.Count - 1) <= 0)
            {
                return;
            }
            var newLayerIndex = layerIndex;
            if (Layers.ElementAtOrDefault(layerIndex) != null)
            {
                RemoveVisualChild(Layers[layerIndex]);
                Layers.RemoveAt(layerIndex);
                RefreshVisualsThenSignalUpdate();
            }
            if (Layers.Count == 0)
            {
                newLayerIndex = -1;
            }
            else if ((layerIndex - 1) >= 0)
            {
                newLayerIndex = layerIndex - 1;
            }
            LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.REMOVELAYER, newLayerIndex));
        }

        /// <summary>
        /// Merges the layer behind the selected one into the selected.
        /// </summary>
        /// <param name="layerIndex">Index of layer selected for merge.</param>
        public void MergeLayerDownward(int layerIndex)
        {
            var newLayerIndex = layerIndex;
            if (Layers.Count >= (layerIndex + 2))
            {
                var bmp = MergeUtils.Merge(Layers, layerIndex + 1, layerIndex);
                Layers[layerIndex].Bitmap = bmp;
                RemoveLayer(Layers[layerIndex + 1]);

                RefreshVisualsThenSignalUpdate();

                LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.MERGELAYER, newLayerIndex));
            }
        }

        /// <summary>
        /// Clears the layer leaving only transparent pixels.
        /// </summary>
        /// <param name="layerIndex">Index of layer to clear.</param>
        public void ClearLayer(int layerIndex)
        {
            if (Layers.Count >= (layerIndex + 1))
            {
                Layers[layerIndex].Clear();

                RefreshVisualsThenSignalUpdate();

                LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.LAYER_PIXELS_CHANGED, layerIndex));
            }
        }

        #endregion Add, Delete, Duplicate, Move Up, Move Down, Merge, Clear
    }
}