using Pixellation.Interfaces;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    internal class DrawingPreview : FrameworkElement
    {
        public List<DrawingLayer> Layers
        {
            get { return (List<DrawingLayer>)GetValue(LayersProperty); }
            set { SetValue(LayersProperty, value); }
        }

        public static readonly DependencyProperty LayersProperty =
         DependencyProperty.Register("Layers", typeof(List<DrawingLayer>), typeof(DrawingPreview), new FrameworkPropertyMetadata(
             new List<DrawingLayer>()
        ));

        public DrawingPreview()
        {
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            PixelEditor.RaiseImageUpdatedEvent += (s, a) =>
            {
                InvalidateVisual();
            };
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (Layers.Count > 0)
            {
                var tmp = Layers[0];
                double w, h;
                if (tmp.MagnifiedWidth >= tmp.MagnifiedHeight && tmp.MagnifiedWidth > Width)
                {
                    w = Width;
                    h = (w / tmp.MagnifiedWidth) * tmp.MagnifiedHeight;
                }
                else if (tmp.MagnifiedHeight >= tmp.MagnifiedWidth && tmp.MagnifiedHeight > Height)
                {
                    h = Height;
                    w = (h / tmp.MagnifiedHeight) * tmp.MagnifiedWidth;
                }
                else
                {
                    w = tmp.MagnifiedWidth;
                    h = tmp.MagnifiedHeight;
                }
                double x = (Width - w) / 2;
                double y = (Height - h) / 2;
                foreach (var l in Layers)
                {
                    var a = l.Bitmap.Clone();
                    a.BlitRender(l.Bitmap, false, (float)l.Opacity);
                    dc.DrawImage(a, new Rect(x, y, w, h));
                }
            }
        }
    }
}