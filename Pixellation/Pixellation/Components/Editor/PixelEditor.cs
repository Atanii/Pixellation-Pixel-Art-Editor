using Pixellation.Components.Tools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor : FrameworkElement, IPreviewable
    {
        private DrawingLayer _activeLayer;
        private Visual _gridLines;
        private Visual _borderLine;

        public List<DrawingLayer> Layers { get; private set; }

        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        public int Magnification { get; set; }

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

        private readonly VisualManager _vm;

        public IVisualManager VisualAndLayerManager => _vm;

        public PixelEditor()
        {
            PixelWidth = 32;
            PixelHeight = 32;
            Magnification = 1;
            _vm = new VisualManager(this);
            Init();
        }

        public PixelEditor(int width = 32, int height = 32, int defaultMagnification = 1)
        {
            PixelWidth = width;
            PixelHeight = height;
            Magnification = defaultMagnification;
            _vm = new VisualManager(this);
            Init();
        }

        private void Init(WriteableBitmap imageToEdit = null)
        {
            var layers = new List<DrawingLayer>();
            if (imageToEdit == null)
            {
                layers.Add(new DrawingLayer(this, "default"));
            }
            else
            {
                layers.Add(new DrawingLayer(this, imageToEdit, "default"));
            }

            _gridLines = CreateGridLines();
            _borderLine = CreateBorderLines();

            Cursor = Cursors.Pen;

            BaseTool.RaiseToolEvent += HandleToolEvent;
            RaiseToolChangeEvent += (d, e) => { UpdateToolProperties(); };

            _vm.SetVisuals(layers, _gridLines, _borderLine);

            _vm.VisualsChanged += (a, b) => { UpdateVisualRelated(); };
        }

        public void NewImage(int width = 32, int height = 32, int defaultMagnification = 1, WriteableBitmap imageToEdit = null)
        {
            PixelWidth = width;
            PixelHeight = height;
            Magnification = defaultMagnification;

            _vm.FlushLayers();

            Init(imageToEdit);

            UpdateToolProperties();

            InvalidateMeasure();
            InvalidateVisual();
        }

        private void UpdateVisualRelated()
        {
            InvalidateVisual();
            ChosenTool.SetActiveLayer(_activeLayer);
        }

        private void UpdateToolProperties()
        {
            ChosenTool.SetDrawingCircumstances(Magnification, PixelWidth, PixelHeight, _activeLayer);
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

        protected override int VisualChildrenCount => _vm.VisualCount;

        protected override Visual GetVisualChild(int index) => _vm.GetVisualChild(index);

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

            _activeLayer?.Measure(size);

            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _activeLayer?.Arrange(new Rect(finalSize));
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

            _activeLayer.InvalidateVisual();

            RenderTransformOrigin = new Point(ActualWidth, ActualHeight);
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            UpdateToolProperties();

            InvalidateVisual();
        }

        public WriteableBitmap GetWriteableBitmap()
        {
            return this._activeLayer.GetWriteableBitmap();
        }

        public ImageSource GetImageSource() => _vm.GetAllMergedImageSource();
    }
}