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

        public event EventHandler RaiseImageUpdatedEvent;

        public override string ToString()
        {
            return LayerName;
        }
    }
}