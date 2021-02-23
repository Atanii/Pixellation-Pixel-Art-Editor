using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    internal class FramePreview : FrameworkElement
    {
        public DrawingFrame Frame { get; set; }

        public FramePreview()
        {
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            PixelEditor.RaiseImageUpdatedEvent += (s, a) =>
            {
                InvalidateVisual();
            };
        }

        [System.Obsolete]
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (Frame != null && Frame.Layers.Count > 0)
            {
                var tmp = Frame.Layers[0];
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
                foreach (var l in Frame.Layers)
                {
                    var a = l.Bitmap.Clone();
                    a.BlitRender(l.Bitmap, false, (float)l.Opacity);
                    dc.DrawImage(a, new Rect(x, y, w, h));
                }

                dc.DrawText(
                    new FormattedText(
                        Frame.FrameName,
                        new System.Globalization.CultureInfo("en-US"),
                        FlowDirection.LeftToRight,
                        new Typeface("Verdana"), 10,
                        new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                    ),
                    new Point(0, Height - 25)
                );
            }
        }
    }
}