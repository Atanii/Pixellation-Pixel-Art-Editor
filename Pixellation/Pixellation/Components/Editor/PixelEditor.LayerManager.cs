using Pixellation.Interfaces;
using Pixellation.Models;
using Pixellation.Properties;
using Pixellation.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor : IVisualManager
    {
        public int VisualCount { get; set; } = 0;

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
        }

        public void ResetLayerAndHelperVisuals(bool includeHelperVisuals = true)
        {
            // Layers
            foreach (var l in Layers)
            {
                RemoveVisualChild(l);
            }
            foreach (var l in Layers)
            {
                AddVisualChild(l);
            }
            // Previewlayer
            ResetPreviewLayer();
            // Helpervisuals
            if (includeHelperVisuals)
            {
                ResetHelperVisuals();
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

        public void SetActiveLayer(int layerIndex = 0)
        {
            if (Layers.ElementAtOrDefault(layerIndex) != null)
            {
                _activeLayer = Layers[layerIndex];

                RefreshVisualsThenSignalUpdate();
            }
        }

        #region Getters
        public WriteableBitmap GetWriteableBitmap() => _activeLayer.GetWriteableBitmap();

        public List<DrawingLayer> GetLayers() => Layers;

        public DrawingLayer GetLayer(int layerIndex = 0) => Layers.ElementAtOrDefault(layerIndex);

        public List<LayerModel> GetLayerModels()
        {
            var list = new List<LayerModel>();
            foreach (var l in Layers)
            {
                list.Add(l.ToLayerModel());
            }
            return list;
        }

        public int GetIndex(DrawingLayer layer)
        {
            return Layers.FindIndex(x => x.LayerName == layer.LayerName);
        }
        #endregion Getters

        #region Merge
        /// <summary>
        /// Merges the layers in the given index range into a single WriteableBitmap.
        /// The indexing is reverse!
        /// </summary>
        /// <param name="from">From index relative to last layer index</param>
        /// <param name="to">To index relative to last layer index. Default is 0, which means the layer above all others.</param>
        /// <returns>The bitmap containing the merged layers. If no merge could have been done in the range, a blank bitmap will be returned.</returns>
        public WriteableBitmap Merge(int from, int to = 0, WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.Alpha)
        {
            // Blank bitmap as mergebase
            var merged = BitmapFactory.New(PixelWidth, PixelHeight);
            merged.Clear(Colors.Transparent);

            var rect = new System.Windows.Rect(0d, 0d, merged.Width, merged.Height); ;

            for (int i = from; i >= to; i--)
            {
                merged.Blit(rect, Layers[i].GetWriteableBitmapWithAppliedOpacity(), rect, mode);
            }

            return merged;
        }

        public WriteableBitmap GetAllMergedWriteableBitmap() => Merge(Layers.Count() - 1, 0);

        public ImageSource GetImageSource() => Merge(Layers.Count() - 1, 0).ToImageSource();
        #endregion Merge

        #region Transform
        public void Mirror(bool horizontally, bool allLayers)
        {
            _mementoCaretaker.Clear();

            if (!allLayers && _activeLayer != null)
            {
                _activeLayer.Mirror(horizontally);
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

        public void Rotate(int angleInDegree, bool allLayers, bool counterClockWise)
        {
            _mementoCaretaker.Clear();

            if (!allLayers && _activeLayer != null)
            {
                _activeLayer.Rotate(counterClockWise ? 360 - angleInDegree : angleInDegree);
            }
            else
            {
                foreach (var l in Layers)
                {
                    l.Rotate(counterClockWise ? 360 - angleInDegree : angleInDegree);
                }
            }
            RefreshVisualsThenSignalUpdate();
        }

        public void Resize(int newWidth, int newHeight)
        {
            _mementoCaretaker.Clear();

            foreach (var l in Layers)
            {
                l.Resize(newWidth, newHeight);
            }

            PixelWidth = newWidth;
            PixelHeight = newHeight;
            Magnification = Settings.Default.DefaultMagnification;
        }
        #endregion Transform

        #region Add, Delete, Duplicate, Move Up, Move Down, Merge
        public int AddLayer(DrawingLayer layer, int layerIndex = 0)
        {
            Layers.Insert(layerIndex, layer);
            AddVisualChild(Layers[layerIndex]);

            RefreshVisualsThenSignalUpdate();

            return layerIndex;
        }

        public int AddLayer(string name, int layerIndex = 0)
        {
            Layers.Insert(layerIndex, new DrawingLayer(this, name));
            AddVisualChild(Layers[layerIndex]);

            RefreshVisualsThenSignalUpdate();

            return layerIndex;
        }

        public int DuplicateLayer(int layerIndex = 0)
        {
            Layers.Insert(layerIndex, Layers[layerIndex].Clone());
            AddVisualChild(Layers[layerIndex]);

            RefreshVisualsThenSignalUpdate();

            return layerIndex;
        }

        public int MoveLayerUp(int layerIndex)
        {
            if (layerIndex > 0)
            {
                var tmp = Layers[layerIndex];
                Layers.RemoveAt(layerIndex);
                Layers.Insert(--layerIndex, tmp);
                RefreshVisualsThenSignalUpdate();
            }
            return layerIndex;
        }

        public int MoveLayerDown(int layerIndex)
        {
            if (layerIndex < Layers.Count() - 1)
            {
                var tmp = Layers[layerIndex];
                Layers.RemoveAt(layerIndex);
                Layers.Insert(++layerIndex, tmp);
                RefreshVisualsThenSignalUpdate();
            }
            return layerIndex;
        }

        public void RemoveLayer(DrawingLayer layer)
        {
            _mementoCaretaker.Clear();
            RemoveVisualChild(layer);
            Layers.Remove(layer);
            RefreshVisualsThenSignalUpdate();
        }

        public int RemoveLayer(int layerIndex)
        {
            _mementoCaretaker.Clear();
            if (Layers.ElementAtOrDefault(layerIndex) != null)
            {
                RemoveVisualChild(Layers[layerIndex]);
                Layers.RemoveAt(layerIndex);
                RefreshVisualsThenSignalUpdate();
            }
            if (Layers.Count == 0)
            {
                return -1;
            }
            else if ((layerIndex - 1) >= 0)
            {
                return layerIndex - 1;
            }
            else
            {
                return layerIndex;
            }
        }

        public int MergeLayerDownward(int layerIndex)
        {
            _mementoCaretaker.Clear();
            if (Layers.Count >= (layerIndex + 2))
            {
                var bmp = Merge(layerIndex + 1, layerIndex);
                Layers[layerIndex].SetBitmap(bmp);
                RemoveLayer(layerIndex + 1);
                return layerIndex;
            }
            return -1;
        }
        #endregion Add, Delete, Duplicate, Move Up, Move Down
    }
}