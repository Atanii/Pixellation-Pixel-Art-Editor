using Pixellation.Models;
using Pixellation.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor
    {
        public interface IVisualManager
        {
            public event EventHandler VisualsChanged;

            public void SetActiveLayer(int layerIndex = 0);

            public List<DrawingLayer> GetLayers();

            public DrawingLayer GetLayer(int layerIndex = 0);

            public void AddLayer(DrawingLayer layer, int layerIndex = 0);

            public void AddLayer(string name, int layerIndex = 0);

            public void RemoveLayer(DrawingLayer layer);

            public void RemoveLayer(int layerIndex);

            public WriteableBitmap Merge(int from, int to = 0, WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.Alpha);

            public DrawingLayer GetAllMerged();

            public WriteableBitmap GetAllMergedWriteableBitmap();

            public ImageSource GetAllMergedImageSource();

            public Bitmap GetAllMergedBitmap();

            public void MoveUp(int layerIndex);

            public void MoveDown(int layerIndex);

            public List<LayerModel> GetLayerModels();
        }

        private class VisualManager : IVisualManager
        {
            private readonly PixelEditor _pe;
            public List<DrawingLayer> Layers { get; set; }

            public event EventHandler VisualsChanged;

            public int VisualCount = 0;

            public VisualManager(PixelEditor pe)
            {
                _pe = pe;
            }

            public VisualManager(PixelEditor pe, List<DrawingLayer> layers, Visual grid = null, Visual lines = null, DrawingLayer drawPreview = null)
            {
                _pe = pe;
                SetVisuals(layers, grid, lines, drawPreview);
            }

            public void SetVisuals(List<DrawingLayer> layers, Visual grid = null, Visual lines = null, DrawingLayer drawPreview = null)
            {
                Layers = layers;

                // layers
                for (int i = 0; i < Layers.Count(); i++)
                {
                    var tmp = Layers[i];
                    _pe.AddVisualChild(tmp);
                    if (i == 0)
                        _pe._activeLayer = tmp;
                    ++VisualCount;
                }

                if (drawPreview != null)
                {
                    _pe.AddVisualChild(drawPreview);
                    ++VisualCount;
                }
                // decoration goes last
                if (grid != null)
                {
                    _pe.AddVisualChild(grid);
                    ++VisualCount;
                }
                if (lines != null)
                {
                    _pe.AddVisualChild(lines);
                    ++VisualCount;
                }                

                VisualsChanged?.Invoke(this, EventArgs.Empty);
            }

            public void InvalidateAllLayerVisual()
            {
                foreach (var l in Layers)
                {
                    l.InvalidateMeasure();
                    l.InvalidateVisual();
                }
            }

            public void MeasureAllLayer(System.Windows.Size s)
            {
                foreach (var l in Layers)
                {
                    l?.Measure(s);
                }
                if (_pe._drawPreview != null)
                {
                    _pe._drawPreview.Measure(s);
                }
            }

            public void ArrangeAllLayer(System.Windows.Rect s)
            {
                foreach (var l in Layers)
                {
                    l?.Arrange(s);
                }
                if (_pe._drawPreview != null)
                {
                    _pe._drawPreview.Arrange(s);
                }
            }

            public void DeleteAllVisualChildren()
            {
                foreach (var l in Layers)
                {
                    _pe.RemoveVisualChild(l);
                }
                _pe.RemoveVisualChild(_pe._drawPreview);
                _pe.RemoveVisualChild(_pe._gridLines);
                _pe.RemoveVisualChild(_pe._borderLine);
                Layers.Clear();
            }

            public Visual GetVisualChild(int index)
            {
                if (index < Layers.Count())
                {
                    return Layers[(Layers.Count() - 1) - index];
                }
                else if (index == VisualCount - 3)
                {
                    return _pe._drawPreview;
                }
                else if (index == VisualCount - 2)
                {
                    return _pe._gridLines;
                }
                else if (index == VisualCount - 1)
                {
                    return _pe._borderLine;
                }
                else
                {
                    return null;
                }
            }

            public void RefreshMiscVisuals()
            {
                _pe.RemoveVisualChild(_pe._drawPreview);
                _pe.RemoveVisualChild(_pe._gridLines);
                _pe.RemoveVisualChild(_pe._borderLine);

                _pe.AddVisualChild(_pe._drawPreview);
                _pe.AddVisualChild(_pe._gridLines);
                _pe.AddVisualChild(_pe._borderLine);
            }

            public void RefreshLayerVisuals(bool miscToo = true)
            {
                foreach (var l in Layers)
                {
                    _pe.RemoveVisualChild(l);
                }
                foreach (var l in Layers)
                {
                    _pe.AddVisualChild(l);
                }
                if (miscToo)
                    RefreshMiscVisuals();
            }

            public void SetActiveLayer(int layerIndex = 0)
            {
                if (Layers.ElementAtOrDefault(layerIndex) != null)
                {
                    _pe._activeLayer = Layers[layerIndex];

                    VisualsChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public List<DrawingLayer> GetLayers() => Layers;

            public DrawingLayer GetLayer(int layerIndex = 0) => Layers.ElementAtOrDefault(layerIndex);

            public void AddLayer(DrawingLayer layer, int layerIndex = 0)
            {
                Layers.Insert(layerIndex, layer);
                _pe.AddVisualChild(Layers[layerIndex]);
                ++VisualCount;

                RefreshMiscVisuals();

                VisualsChanged?.Invoke(this, EventArgs.Empty);
            }

            public void AddLayer(string name, int layerIndex = 0)
            {
                Layers.Insert(layerIndex, new DrawingLayer(_pe, name));
                _pe.AddVisualChild(Layers[layerIndex]);
                ++VisualCount;

                RefreshMiscVisuals();

                VisualsChanged?.Invoke(this, EventArgs.Empty);
            }

            public void MoveUp(int layerIndex)
            {
                if (layerIndex > 0)
                {
                    var tmp = Layers[layerIndex];
                    Layers.RemoveAt(layerIndex);
                    Layers.Insert(--layerIndex, tmp);
                    RefreshLayerVisuals();
                    VisualsChanged?.Invoke(this, EventArgs.Empty);
                }                
            }

            public void MoveDown(int layerIndex)
            {
                if (layerIndex < Layers.Count() - 1)
                {
                    var tmp = Layers[layerIndex];
                    Layers.RemoveAt(layerIndex);
                    Layers.Insert(++layerIndex, tmp);
                    RefreshLayerVisuals();
                    VisualsChanged?.Invoke(this, EventArgs.Empty);
                }
            }

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
                var merged = BitmapFactory.New(_pe.PixelWidth, _pe.PixelHeight);
                merged.Clear(Colors.Transparent);

                var rect = new System.Windows.Rect(0d, 0d, merged.Width, merged.Height); ;

                for (int i = from; i >= to; i--)
                {
                    merged.Blit(rect, Layers[i].GetWriteableBitmapWithAppliedOpacity(), rect, mode);
                }

                return merged;
            }

            public DrawingLayer GetAllMerged()
            {
                var merged = Merge(Layers.Count() - 1, 0);
                return new DrawingLayer(_pe, merged, "merged");
            }

            public WriteableBitmap GetAllMergedWriteableBitmap() => Merge(Layers.Count() - 1, 0);

            public Bitmap GetAllMergedBitmap() => Merge(Layers.Count() - 1, 0).ToBitmap();

            public ImageSource GetAllMergedImageSource() => Merge(Layers.Count() - 1, 0).ToImageSource();

            public void RemoveLayer(DrawingLayer layer)
            {
                _pe.RemoveVisualChild(layer);
                Layers.Remove(layer);
                --VisualCount;

                VisualsChanged?.Invoke(this, EventArgs.Empty);
            }

            public void RemoveLayer(int layerIndex)
            {
                if (Layers.ElementAtOrDefault(layerIndex) != null)
                {
                    _pe.RemoveVisualChild(Layers[layerIndex]);
                    Layers.RemoveAt(layerIndex);
                    --VisualCount;

                    VisualsChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public List<LayerModel> GetLayerModels()
            {
                var list = new List<LayerModel>();
                foreach (var l in Layers)
                {
                    list.Add(l.ToLayerModel());
                }
                return list;
            }
        }
    }
}