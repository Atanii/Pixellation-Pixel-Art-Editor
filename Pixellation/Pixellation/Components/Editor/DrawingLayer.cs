using Pixellation.Models;
using Pixellation.Utils;
using System;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class DrawingLayer : DrawingSurface, IPreviewable
    {
        private string _name;
        public string LayerName {
            get { return _name; }
            set { _name = value; _owner.UpdateVisualRelated(); }
        }
        public bool Visible { get; set; }

        public DrawingLayer(PixelEditor owner, string layerName = "", bool visible = true) : base(owner)
        {
            if (layerName == "")
            {
                _name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                _name = layerName;
            }
            Visible = visible;
        }

        public DrawingLayer(PixelEditor owner, WriteableBitmap bitmap, string layerName = "", bool visible = true) : base(owner, bitmap)
        {
            if (layerName == "")
            {
                _name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                _name = layerName;
            }
            Visible = visible;
        }

        public DrawingLayer(PixelEditor owner, LayerModel model, bool visible = true) : base(
            owner,
            model.LayerBitmap.ToWriteableBitmap(model.Width, model.Height, model.Stride)
        ) {
            if (model.LayerName == "")
            {
                _name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                _name = model.LayerName;
            }
            Visible = visible;
            Opacity = model.Opacity;
        }

        public event EventHandler RaiseImageUpdatedEvent;

        public override string ToString()
        {
            return LayerName;
        }

        public LayerModel ToLayerModel()
        {
            var src = _bitmap.ToImageSource();

            var width = _bitmap.PixelWidth;
            var height = _bitmap.PixelHeight;
            var stride = _bitmap.BackBufferStride;

            var bitmapData = new byte[height * stride];

            src.CopyPixels(bitmapData, stride, 0);

            return new LayerModel
            {
                LayerBitmap = bitmapData,
                LayerName = _name,
                Width = width,
                Height = height,
                Stride = stride,
                Opacity = Opacity
            };
        }
    }
}