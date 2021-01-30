using System;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class DrawingLayer : DrawingSurface, IPreviewable
    {
        private string name;
        public string LayerName {
            get { return name; }
            set { name = value; _owner.UpdateVisualRelated(); }
        }
        public bool Visible { get; set; }

        public DrawingLayer(PixelEditor owner, string layerName = "", bool visible = true) : base(owner)
        {
            if (layerName == "")
            {
                name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                name = layerName;
            }
            Visible = visible;
        }

        public DrawingLayer(PixelEditor owner, WriteableBitmap bitmap, string layerName = "", bool visible = true) : base(owner, bitmap)
        {
            if (layerName == "")
            {
                name = "Layer-" + (new DateTime()).Ticks;
            }
            else
            {
                name = layerName;
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