using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class DrawingFrame : FrameworkElement
    {
        public static readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Color.FromArgb(25, 50, 50, 50));
        public static readonly Pen BackgroundPen = new Pen(BackgroundBrush, 1d);
        public static readonly Color NameDrawColor = Color.FromArgb(255, 0, 0, 0);

        public List<DrawingLayer> Layers { get; private set; }
        public string FrameName { get; private set; }

        private readonly PixelEditor _owner;

        public bool Visible { get; set; }

        public new double Opacity
        {
            get
            {
                if (Visible)
                {
                    return base.Opacity;
                }
                return base.Opacity;
            }
            set
            {
                base.Opacity = value;
            }
        }

        public int MagnifiedWidth => _owner.PixelWidth * _owner.Magnification;
        public int MagnifiedHeight => _owner.PixelHeight * _owner.Magnification;


        public DrawingFrame(List<DrawingLayer> layers, string name, PixelEditor owner, bool visible = true) : base()
        {
            _owner = owner;
            Layers = layers;
            FrameName = name;
            Name = name;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            PixelEditor.RaiseImageUpdatedEvent += (s, a) =>
            {
                InvalidateVisual();
            };
            Visible = visible;
        }

        public DrawingFrame(string name, PixelEditor owner, bool visible = true)
        {
            _owner = owner;
            Layers = new List<DrawingLayer>() { new DrawingLayer(_owner, "Default") };
            FrameName = name;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            PixelEditor.RaiseImageUpdatedEvent += (s, a) =>
            {
                InvalidateVisual();
            };
            Visible = visible;
        }

        private static void DrawBackground(DrawingContext dc, double x, double y, double width, double height)
        {
            dc.DrawRectangle(
                BackgroundBrush, BackgroundPen,
                new Rect(
                    x, y, width, height
                )
            );
        }

        private void DrawLayers(DrawingContext dc, double x, double y, double width, double height)
        {
            foreach (var l in Layers)
            {
                if (!l.Visible)
                {
                    continue;
                }
                var bmp = l.Bitmap.Clone();
                bmp.BlitRender(l.Bitmap, false, (float)l.Opacity);
                dc.DrawImage(bmp, new Rect(x, y, width, height));
            }
        }

        private void DrawName(DrawingContext dc, double x, double y, Color c)
        {
            dc.DrawText(
                new FormattedText(
                    FrameName,
                    new System.Globalization.CultureInfo("en-US"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"), 18,
                    new SolidColorBrush(c),
                    VisualTreeHelper.GetDpi(this).PixelsPerDip
                ),
                new Point(x, y)
            );
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (!Visible)
            {
                return;
            }

            DrawBackground(dc, 0, 0, Width, Height);

            if (Layers != null && Layers.Count > 0)
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

                DrawLayers(dc, x, y, w, h);
            }

            DrawName(dc, 0, Height + 10, NameDrawColor);
        }

        public void Render(DrawingContext dc, double x, double y, double w, double h, bool drawName = false)
        {
            if (!Visible)
            {
                return;
            }

            if (Layers != null && Layers.Count > 0)
            {
                DrawLayers(dc, x, y, w, h);
            }

            if (drawName)
            {
                DrawName(dc, 0, h + 10, NameDrawColor);
            }
        }

        public DrawingFrame Clone()
        {
            var layers = new List<DrawingLayer>();
            foreach (var l in Layers)
            {
                layers.Add(l.Clone());
            }
            return new DrawingFrame(layers, FrameName, _owner);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Debug.WriteLine("CLICK2");
            _owner.SetActiveFrame(this);
        }
    }
}