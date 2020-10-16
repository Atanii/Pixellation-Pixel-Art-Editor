using Pixellation.Utils;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class PixelEditor : FrameworkElement
    {
        private readonly Surface _surface;
        private Visual _gridLines;
        private Visual _borderLine;

        public int PixelWidth { get; } = 32;
        public int PixelHeight { get; } = 32;
        public int Magnification { get; set; } = 5;

        public System.Drawing.Color ChosenColour
        {
            get { return (System.Drawing.Color)GetValue(ChosenColourProperty); }
            set { SetValue(ChosenColourProperty, value); }
        }
        public static readonly DependencyProperty ChosenColourProperty =
         DependencyProperty.Register("ChosenColour", typeof(System.Drawing.Color), typeof(PixelEditor), new FrameworkPropertyMetadata(
            System.Drawing.Color.Black));

        public PixelEditor()
        {
            _surface = new Surface(this);
            _gridLines = CreateGridLines();
            _borderLine = CreateBorderLines();

            Cursor = Cursors.Pen;

            AddVisualChild(_surface);
            AddVisualChild(_gridLines);
            AddVisualChild(_borderLine);
        }

        public PixelEditor(int width, int height, int defaultMagnification = 1)
        {
            PixelWidth = width;
            PixelHeight = height;
            Magnification = defaultMagnification;

            _surface = new Surface(this);
            _gridLines = CreateGridLines();
            _borderLine = CreateBorderLines();

            Cursor = Cursors.Pen;

            AddVisualChild(_surface);
            AddVisualChild(_gridLines);
            AddVisualChild(_borderLine);
        }

        protected override int VisualChildrenCount => 3;

        protected override Visual GetVisualChild(int index)
        {
            return index switch
            {
                0 => _surface,
                1 => _gridLines,
                2 => _borderLine,
                _ => null,
            };
        }

        private void Draw()
        {
            var p = Mouse.GetPosition(_surface);
            var magnification = Magnification;
            var surfaceWidth = PixelWidth * magnification;
            var surfaceHeight = PixelHeight * magnification;

            if (p.X < 0 || p.X >= surfaceWidth || p.Y < 0 || p.Y >= surfaceHeight)
                return;

            _surface.SetColor(
                (int)(p.X / magnification),
                (int)(p.Y / magnification),
                ExtensionMethods.ToMediaColor(ChosenColour));

            _surface.InvalidateVisual();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
                Draw();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            CaptureMouse();
            Draw();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var magnification = Magnification;
            var size = new Size(PixelWidth * magnification, PixelHeight * magnification);

            _surface.Measure(size);

            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _surface.Arrange(new Rect(finalSize));
            return finalSize;
        }

        private Visual CreateGridLines()
        {
            var dv = new DrawingVisual();
            var dc = dv.RenderOpen();

            var w = PixelWidth;
            var h = PixelHeight;
            var m = Magnification;
            var d = -0.5d; // snap gridlines to device pixels

            var pen = new Pen(new SolidColorBrush(Color.FromArgb(63, 63, 63, 63)), 1d);

            pen.Freeze();

            for (var x = 1; x < w; x++)
                dc.DrawLine(pen, new Point(x * m + d, 0), new Point(x * m + d, h * m));

            for (var y = 1; y < h; y++)
                dc.DrawLine(pen, new Point(0, y * m + d), new Point(w * m, y * m + d));

            dc.Close();

            return dv;
        }

        private Visual CreateBorderLines()
        {
            var dv = new DrawingVisual();
            var dc = dv.RenderOpen();

            var w = PixelWidth;
            var h = PixelHeight;
            var m = Magnification;
            // var d = -0.5d; // snap gridlines to device pixels

            var pen = new Pen(new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)), 2d);

            pen.Freeze();

            dc.DrawLine(pen, new Point(0, 0), new Point(w * m, 0));
            dc.DrawLine(pen, new Point(0, h * m), new Point(h * m, w * m));

            dc.DrawLine(pen, new Point(0, 0), new Point(0, h * m));
            dc.DrawLine(pen, new Point(w * m, 0), new Point(w * m, h * m));

            dc.Close();

            return dv;
        }

        public WriteableBitmap GetBitmap()
        {
            return this._surface.GetBitMap();
        }

        public void UpdateMagnification(int zoom)
        {
            Magnification = zoom;

            RemoveVisualChild(_gridLines);
            _gridLines = CreateGridLines();

            RemoveVisualChild(_borderLine);
            _borderLine = CreateBorderLines();
            
            AddVisualChild(_borderLine);
            AddVisualChild(_gridLines);

            _surface.InvalidateVisual();
        }

        private sealed class Surface : FrameworkElement
        {
            private readonly PixelEditor _owner;
            private readonly WriteableBitmap _bitmap;

            public Surface(PixelEditor owner)
            {
                _owner = owner;
                _bitmap = BitmapFactory.New(owner.PixelWidth, owner.PixelHeight);
                _bitmap.Clear(Colors.Transparent); // Colors.White
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            }

            protected override void OnRender(DrawingContext dc)
            {
                base.OnRender(dc);

                var magnification = _owner.Magnification;
                var width = _bitmap.PixelWidth * magnification;
                var height = _bitmap.PixelHeight * magnification;

                dc.DrawImage(_bitmap, new Rect(0, 0, width, height));
            }

            internal void SetColor(int x, int y, Color color)
            {
                _bitmap.SetPixel(x, y, color);
            }

            public WriteableBitmap GetBitMap()
            {
                return this._bitmap;
            }
        }
    }
}