using Pixellation.Utils;
using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class DrawingLayer : DrawingSurface, IPreviewable
    {
        public string LayerName { get; set; }
        public bool Visible { get; set; }

        public DrawingLayer(PixelEditor owner, string layerName = "", bool visible = true) : base(owner)
        {   
            if (layerName == "")
            {
                LayerName = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                LayerName = layerName;
            }
            Visible = visible;
        }

        public DrawingLayer(PixelEditor owner, WriteableBitmap bitmap, string layerName = "", bool visible = true) : base(owner, bitmap)
        {
            if (layerName == "")
            {
                LayerName = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                LayerName = layerName;
            }
            Visible = visible;
        }

        public event EventHandler RaiseImageUpdatedEvent;

        public override string ToString()
        {
            return LayerName;
        }

        public BitmapImage GetImageSource(int width = 0, int height = 0)
        {
            var temp = _bitmap.ToBitmap();
            width = width == 0 ? temp.Width : width;
            height = height == 0 ? temp.Height : height;
            var bi = new Bitmap(temp, new Size() { Width = width, Height = height });
            return bi.ToImageSource();
        }
    }
}