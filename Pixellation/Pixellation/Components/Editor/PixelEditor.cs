using Pixellation.Components.Panels;
using Pixellation.Components.Tools;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class PixelEditor : FrameworkElement, IPreviewable
    {
        private DrawingSurface _surface;
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
            System.Drawing.Color.Black
        ));

        public BaseTool ChosenTool
        {
            get { return (BaseTool)GetValue(ChosenToolProperty); }
            set { SetValue(ChosenToolProperty, value); }
        }

        public readonly DependencyProperty ChosenToolProperty =
         DependencyProperty.Register("ChosenTool", typeof(ITool), typeof(PixelEditor), new PropertyMetadata(
             null, (d, e) => { RaiseToolChangeEvent?.Invoke(default, EventArgs.Empty); }
        ));

        private static event EventHandler RaiseToolChangeEvent;

        public event EventHandler RaiseImageUpdatedEvent;

        public PixelEditor()
        {
            Init();
        }

        public PixelEditor(int width, int height, int defaultMagnification = 1)
        {
            PixelWidth = width;
            PixelHeight = height;
            Magnification = defaultMagnification;
            Init();
        }

        private void Init()
        {
            _surface = new DrawingSurface(this);
            _gridLines = CreateGridLines();
            _borderLine = CreateBorderLines();

            Cursor = Cursors.Pen;

            AddVisualChild(_surface);
            AddVisualChild(_gridLines);
            AddVisualChild(_borderLine);

            BaseTool.RaiseToolEvent += HandleToolEvent;
            RaiseToolChangeEvent += HandleToolChangeEvent;
        }

        private void HandleToolChangeEvent(object sender, EventArgs e)
        {
            ChosenTool.SetDrawingCircumstances(Magnification, PixelWidth, PixelHeight, _surface);
        }

        private void HandleToolEvent(object sender, ToolEventArgs e)
        {
            switch (e.Type)
            {
                case ToolEventType.COLOR:
                    ChosenColour = (System.Drawing.Color)e.Value;
                    break;

                case ToolEventType.NOTHING:
                default:
                    break;
            }
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsMouseCaptured)
                ChosenTool.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            CaptureMouse();

            ChosenTool.SetDrawColor(ChosenColour);
            ChosenTool.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
            ChosenTool.OnMouseLeftButtonUp(e);
            RaiseImageUpdatedEvent?.Invoke(this, EventArgs.Empty);
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

        public WriteableBitmap GetBitmap()
        {
            return this._surface.GetBitMap();
        }
    }
}