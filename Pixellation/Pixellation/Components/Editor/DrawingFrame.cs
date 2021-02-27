using Pixellation.Components.Dialogs;
using Pixellation.Interfaces;
using Pixellation.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class DrawingFrame : FrameworkElement, IBitmapProvider
    {
        private readonly Guid _id = Guid.NewGuid();
        public string Id => _id.ToString();

        public static readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Color.FromArgb(25, 50, 50, 50));
        public static readonly Pen BackgroundPen = new Pen(BackgroundBrush, 1d);

        public static readonly SolidColorBrush BorderBrush = new SolidColorBrush(Color.FromArgb(255, 10, 10, 10));
        public static readonly Pen BorderPen = new Pen(BorderBrush, 1d);

        public static readonly Color NameDrawColor = Color.FromArgb(255, 0, 0, 0);

        public List<DrawingLayer> Layers { get; private set; } = new List<DrawingLayer>() { };

        private readonly IDrawingHelper _owner;

        private string _name;
        public string FrameName
        {
            get => _name;
            set
            {
                _name = value;
                OnUpdated?.Invoke();
            }
        }

        public new double Opacity
        {
            get
            {
                if (Visible)
                {
                    return base.Opacity;
                }
                return 0d;
            }
            set
            {
                base.Opacity = value;
                InvalidateVisual();
                OnUpdated?.Invoke();
            }
        }

        private bool _visible = true;
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                InvalidateVisual();
                OnUpdated?.Invoke();
            }
        }

        public WriteableBitmap Bitmap
        {
            get => MergeAndExportUtils.MergeAll(Layers);
        }

        public int MagnifiedWidth => _owner.PixelWidth * _owner.Magnification;
        public int MagnifiedHeight => _owner.PixelHeight * _owner.Magnification;

        #region Events

        public static event FrameEventHandler OnUpdated;

        #endregion Events

        public DrawingFrame(List<DrawingLayer> layers, string name, IDrawingHelper owner, bool visible = true, double opacity = 100) : base()
        {
            _owner = owner;
            Layers = layers;
            FrameName = name;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            PixelEditor.RaiseImageUpdatedEvent += (s, a) =>
            {
                InvalidateVisual();
            };
            Visible = visible;
            Opacity = opacity;
        }

        public DrawingFrame(string name, IDrawingHelper owner, bool visible = true, bool generateDefaultLayer = false)
        {
            _owner = owner;
            FrameName = name;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            PixelEditor.RaiseImageUpdatedEvent += (s, a) =>
            {
                InvalidateVisual();
            };
            Visible = visible;
            if (generateDefaultLayer)
            {
                Layers.Add(new DrawingLayer(_owner, "Default"));
            }
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

        private static void DrawBorder(DrawingContext dc, double x, double y, double width, double height)
        {
            dc.DrawRectangle(
                null, BorderPen,
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

        private static void DrawText(DrawingContext dc, double x, double y, Color c, string text, double dpi, int size)
        {
            dc.DrawText(
                new FormattedText(
                    text,
                    new System.Globalization.CultureInfo("en-US"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    size,
                    new SolidColorBrush(c),
                    dpi
                ),
                new Point(x, y)
            );
        }

        private void DrawX(DrawingContext dc)
        {
            dc.DrawLine(
                BorderPen,
                new Point(0, 0),
                new Point(Width, Height)
            );

            dc.DrawLine(
                BorderPen,
                new Point(0, Height),
                new Point(Width, 0)
            );
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (!Visible)
            {
                DrawX(dc);
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

            var dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            DrawText(dc, 0, Height + 10, NameDrawColor, FrameName, dpi, 15);
            DrawText(dc, 0, Height + 25, NameDrawColor, $"Opacity: {Opacity}", dpi, 15);

            if (_owner.ActiveFrameId == Id)
            {
                DrawBorder(dc, -5, -5, Width + 10, Height + 10);
            }
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
                DrawText(dc, 0, h + 10, NameDrawColor, FrameName, VisualTreeHelper.GetDpi(this).PixelsPerDip, 15);
            }
        }

        public DrawingFrame Clone()
        {
            var layers = new List<DrawingLayer>();
            foreach (var l in Layers)
            {
                layers.Add(l.Clone());
            }
            return new DrawingFrame(layers, FrameName + "_copy", _owner);
        }

        public IEnumerable<BitmapSource> GetLayersAsBitmapSources()
        {
            var bmps = new List<WriteableBitmap>();
            foreach(var layer in Layers)
            {
                bmps.Add(layer.GetWriteableBitmapWithAppliedOpacity());
            }
            return bmps;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.RightButton == MouseButtonState.Pressed)
            {
                Visible = !Visible;
                return;
            }
            else
            {
                if (e.ClickCount == 2)
                {
                    var strDoubleDialog = new StringDoubleDialog("Frame Settings", "Name", "Opacity", FrameName, Opacity);
                    if ((bool)strDoubleDialog.ShowDialog())
                    {
                        FrameName = strDoubleDialog.Answer.Key;
                        Opacity = strDoubleDialog.Answer.Value;
                    }
                }
                else
                {
                    _owner.SetActiveFrame(this);
                }
            }
        }

    }
}