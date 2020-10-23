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

            public Bitmap Merge(int from, int to = 0);

            public DrawingLayer GetAllMerged();

            public WriteableBitmap GetAllMergedWriteableBitmap();

            public Bitmap GetAllMergedBitmap();
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

            public VisualManager(PixelEditor pe, List<DrawingLayer> layers, Visual grid = null, Visual lines = null)
            {
                _pe = pe;
                SetVisuals(layers, grid, lines);
            }

            public void SetVisuals(List<DrawingLayer> layers, Visual grid = null, Visual lines = null)
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

            public void FlushLayers()
            {
                Layers.RemoveAll((x) => true);
            }

            public Visual GetVisualChild(int index)
            {
                if (index < Layers.Count())
                {
                    return Layers[index];
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

            public void SetActiveLayer(int layerIndex = 0)
            {
                if (Layers.ElementAtOrDefault(layerIndex) != null)
                {
                    _pe._activeLayer = Layers[layerIndex];

                    VisualsChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public List<DrawingLayer> GetLayers()
            {
                return Layers;
            }

            public DrawingLayer GetLayer(int layerIndex = 0)
            {
                return Layers.ElementAtOrDefault(layerIndex);
            }

            public void AddLayer(DrawingLayer layer, int layerIndex = 0)
            {
                Layers.Insert(layerIndex, layer);
                _pe.AddVisualChild(Layers[layerIndex]);
                ++VisualCount;

                VisualsChanged?.Invoke(this, EventArgs.Empty);
            }

            public void AddLayer(string name, int layerIndex = 0)
            {
                Layers.Insert(layerIndex, new DrawingLayer(_pe, name));
                _pe.AddVisualChild(Layers[layerIndex]);
                ++VisualCount;

                VisualsChanged?.Invoke(this, EventArgs.Empty);
            }

            /// <summary>
            /// Merges the layers in the given index range into a single bitmap.
            /// The indexing is reverse!
            /// </summary>
            /// <param name="from">From index relative to last layer index</param>
            /// <param name="to">To index relative to last layer index. Default is 0, which means the layer above all others.</param>
            /// <returns>The bitmap containing the merged layers. If no merge could have been done in the range, a blank bitmap will be returned.</returns>
            public Bitmap Merge(int from, int to = 0)
            {
                // Blank bitmap as mergebase
                var tmp = BitmapFactory.New(_pe.PixelWidth, _pe.PixelHeight);
                tmp.Clear(Colors.Transparent);
                var merged = tmp.ToBitmap();

                // Merge all layers into one
                using Graphics g = Graphics.FromImage(merged);
                for (int i = from; i >= to; i--)
                {
                    g.DrawImage(Layers[i].GetBitmap(), new Point() { X = 0, Y = 0 });
                }

                // Return merged layers
                return merged;
            }

            public DrawingLayer GetAllMerged()
            {
                var merged = Merge(Layers.Count() - 1, 0);
                return new DrawingLayer(_pe, merged.ToWriteableBitmap(), "merged");
            }

            public WriteableBitmap GetAllMergedWriteableBitmap()
            {
                var merged = Merge(Layers.Count() - 1, 0);
                return merged.ToWriteableBitmap();
            }

            public Bitmap GetAllMergedBitmap()
            {
                var merged = Merge(Layers.Count() - 1, 0);
                return merged;
            }

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
                    _pe.RemoveVisualChild(_pe.GetVisualChild(layerIndex));
                    Layers.RemoveAt(layerIndex);
                    --VisualCount;

                    VisualsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}