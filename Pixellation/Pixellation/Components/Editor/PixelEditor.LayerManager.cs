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
        public static event PixelEditorLayerEventHandler LayerListChanged;

        public DrawingLayer ActiveLayer { get; private set; }

        public int ActiveLayerIndex => Layers.FindIndex(x => x.Id == ActiveLayer.Id);

        /// <summary>
        /// List of <see cref="DrawingLayer"/>s the currently edited image consists of.
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

        public void MeasureAllLayer(System.Windows.Size s)
        {
            foreach (var l in Layers)
            {
                l?.Measure(s);
            }
            if (_drawPreview != null)
            {
                _drawPreview.Measure(s);
            }
        }

        public void ArrangeAllLayer(System.Windows.Rect s)
        {
            foreach (var l in Layers)
            {
                l?.Arrange(s);
            }
            if (_drawPreview != null)
            {
                _drawPreview.Arrange(s);
            }
        }

        public void DeleteAllVisualChildren()
        {
            foreach (var l in Layers)
            {
                RemoveVisualChild(l);
            }
            Layers.Clear();

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

            LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.CLEAR, -1));

            Frames.Clear();

            FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.CLEAR, -1));
        }

        public void ResetLayerAndHelperVisuals(bool includeHelperVisuals = true)
        {
            // Layers
            RemoveLayersFromVisualChildren();
            AddLayersToVisualChildren();

            // Previewlayer
            ResetPreviewLayer();

            // Helpervisuals
            if (includeHelperVisuals)
            {
                ResetHelperVisuals();
            }
        }

        private void RemoveLayersFromVisualChildren()
        {
            foreach (var l in Layers)
            {
                RemoveVisualChild(l);
            }
        }

        private void AddLayersToVisualChildren()
        {
            foreach (var l in Layers)
            {
                AddVisualChild(l);
            }
        }

        public void ResetPreviewLayer()
        {
            if (_drawPreview != null)
            {
                RemoveVisualChild(_drawPreview);
            }
            _drawPreview = new DrawingLayer(this, "DrawPreview");
            AddVisualChild(_drawPreview);
        }

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

        public void SetActiveLayer(int layerIndex = 0, bool signalEvent = false)
        {
            if (Layers.ElementAtOrDefault(layerIndex) != null)
            {
                ActiveLayer = Layers[layerIndex];
                if (signalEvent)
                {
                    LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.NONE, 0));
                }
                RefreshVisualsThenSignalUpdate();
            }
        }

        #region Getters

        public int GetIndex(DrawingLayer layer)
        {
            return Layers.FindIndex(x => x.Id == layer.Id);
        }

        #endregion Getters

        #region Transform

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

        public void Rotate(bool counterClockWise)
        {
            foreach (var l in Layers)
            {
                l.Rotate(counterClockWise ? 270 : 90);
            }

            var tmp = PixelHeight;
            PixelHeight = PixelWidth;
            PixelWidth = tmp;

            UpdateMagnification();
        }

        public void Resize(int newWidth, int newHeight)
        {
            foreach (var l in Layers)
            {
                l.Resize(newWidth, newHeight);
            }

            PixelWidth = newWidth;
            PixelHeight = newHeight;
            Magnification = Settings.Default.DefaultMagnification;
        }

        #endregion Transform

        #region Add, Delete, Duplicate, Move Up, Move Down, Merge, Clear

        public void AddLayer(DrawingLayer layer, int layerIndex = 0)
        {
            Layers.Insert(layerIndex, layer);
            AddVisualChild(Layers[layerIndex]);

            RefreshVisualsThenSignalUpdate();

            LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.ADDLAYER, layerIndex));
        }

        public void AddLayer(string name, int layerIndex = 0)
        {
            Layers.Insert(layerIndex, new DrawingLayer(this, name));
            AddVisualChild(Layers[layerIndex]);

            RefreshVisualsThenSignalUpdate();

            LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.ADDLAYER, layerIndex));
        }

        public void DuplicateLayer(int layerIndex = 0)
        {
            Layers.Insert(layerIndex, Layers[layerIndex].Clone());
            AddVisualChild(Layers[layerIndex]);

            RefreshVisualsThenSignalUpdate();

            LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.DUPLICATELAYER, layerIndex));
        }

        public void MoveLayerUp(int layerIndex)
        {
            var newLayerIndex = layerIndex;
            if (layerIndex > 0)
            {
                var tmp = Layers[layerIndex];
                Layers.RemoveAt(layerIndex);
                --newLayerIndex;
                Layers.Insert(newLayerIndex, tmp);
                RefreshVisualsThenSignalUpdate();
            }
            LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.MOVELAYERUP, newLayerIndex));
        }

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
            LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.MOVELAYERDOWN, newLayerIndex));
        }

        private void RemoveLayer(DrawingLayer layer)
        {
            RemoveVisualChild(layer);
            Layers.Remove(layer);
        }

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
            LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.REMOVELAYER, newLayerIndex));
        }

        public void MergeLayerDownward(int layerIndex)
        {
            var newLayerIndex = layerIndex;
            if (Layers.Count >= (layerIndex + 2))
            {
                var bmp = MergeUtils.Merge(Layers, layerIndex + 1, layerIndex);
                Layers[layerIndex].Bitmap = bmp;
                RemoveLayer(Layers[layerIndex + 1]);

                RefreshVisualsThenSignalUpdate();

                LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.MERGELAYER, newLayerIndex));
            }
        }

        public void ClearLayer(int layerIndex)
        {
            if (Layers.Count >= (layerIndex + 1))
            {
                Layers[layerIndex].Clear();

                RefreshVisualsThenSignalUpdate();

                LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.LAYER_PIXELS_CHANGED, layerIndex));
            }
        }

        #endregion Add, Delete, Duplicate, Move Up, Move Down, Merge, Clear
    }
}