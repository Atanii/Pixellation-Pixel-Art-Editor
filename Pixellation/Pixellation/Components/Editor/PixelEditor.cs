using Pixellation.Components.Tools;
using Pixellation.Models;
using Pixellation.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor : FrameworkElement, IPreviewable, INotifyPropertyChanged
    {
        private DrawingLayer _activeLayer;
        private Visual _gridLines;
        private Visual _borderLine;
        private DrawingLayer _drawPreview;

        private int _pixelWidth;
        public int PixelWidth
        {
            get { return _pixelWidth; }
            private set
            {
                _pixelWidth = value;
                OnPropertyChanged();
            }
        }

        private int _pixelHeight;
        public int PixelHeight
        {
            get { return _pixelHeight; }
            private set
            {
                _pixelHeight = value;
                OnPropertyChanged();
            }
        }

        private int _magnification;
        public int Magnification
        {
            get { return _magnification; }
            set
            {
                _magnification = value;
                RefreshMeasureAndMagnification();
                OnPropertyChanged();
            }
        }

        public bool Tiled { get; private set; } = false;
        public float TiledOpacity { get; private set; } = Settings.Default.DefaultTiledOpacity;
        public List<DrawingLayer> Layers { get; private set; }

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
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly VisualManager _vm;
        public VisualManager VisualAndLayerManager => _vm;

        public PixelEditor()
        {
            Magnification = Settings.Default.DefaultMagnification;
            _vm = new VisualManager(this);
            Init(Settings.Default.DefaultImageSize, Settings.Default.DefaultImageSize);
            RefreshMeasureAndMagnification();
        }

        public PixelEditor(int width, int height, int defaultMagnification)
        {
            Magnification = defaultMagnification;
            _vm = new VisualManager(this);
            Init(width, height);
            RefreshMeasureAndMagnification();
        }

        private void Init(int width, int height)
        {
            PixelWidth = width;
            PixelHeight = height;

            var layers = new List<DrawingLayer>();
            layers.Add(new DrawingLayer(this, "default"));

            _gridLines = CreateGridLines();
            _borderLine = CreateBorderLines();
            _drawPreview = new DrawingLayer(this, "DrawPreview");

            Cursor = Cursors.Pen;

            BaseTool.RaiseToolEvent += HandleToolEvent;
            RaiseToolChangeEvent += (d, e) => { UpdateToolProperties(); };

            _vm.SetVisuals(layers, _gridLines, _borderLine, _drawPreview);

            _vm.VisualsChanged += (a, b) => { UpdateVisualRelated(); };
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
            _drawPreview = new DrawingLayer(this, "DrawPreview");

            Cursor = Cursors.Pen;

            BaseTool.RaiseToolEvent += HandleToolEvent;
            RaiseToolChangeEvent += (d, e) => { UpdateToolProperties(); };

            _vm.SetVisuals(layers, _gridLines, _borderLine, _drawPreview);

            _vm.VisualsChanged += (a, b) => { UpdateVisualRelated(); };
        }

        private void Init(List<DrawingLayer> layers)
        {
            _gridLines = CreateGridLines();
            _borderLine = CreateBorderLines();
            _drawPreview = new DrawingLayer(this, "DrawPreview");

            Cursor = Cursors.Pen;

            BaseTool.RaiseToolEvent += HandleToolEvent;
            RaiseToolChangeEvent += (d, e) => { UpdateToolProperties(); };

            _vm.SetVisuals(layers, _gridLines, _borderLine, _drawPreview);

            _vm.VisualsChanged += (a, b) => { UpdateVisualRelated(); };
        }

        public void ToggleTiled()
        {
            Tiled = !Tiled;
            _vm.InvalidateAllLayerVisual();
        }

        public void SetTiledOpacity(float newOpacity)
        {
            TiledOpacity = newOpacity;
            _vm.InvalidateAllLayerVisual();
        }

        public void NewImage(int width = 32, int height = 32)
        {
            PixelWidth = width;
            PixelHeight = height;

            _vm.DeleteAllVisualChildren();

            Init();

            RefreshMeasureAndMagnification();
        }

        public void NewImage(WriteableBitmap imageToEdit)
        {
            PixelWidth = imageToEdit.PixelWidth;
            PixelHeight = imageToEdit.PixelHeight;

            _vm.DeleteAllVisualChildren();

            Init(imageToEdit);

            UpdateToolProperties();

            InvalidateMeasure();
            InvalidateVisual();
        }

        public void NewImage(List<LayerModel> models, int width = 32, int height = 32)
        {
            PixelWidth = width;
            PixelHeight = height;

            _vm.DeleteAllVisualChildren();

            var layers = new List<DrawingLayer>();
            foreach(var model in models)
            {
                layers.Add(new DrawingLayer(
                    this,
                    model
                ));
            }

            Init(layers);

            RefreshMeasureAndMagnification();
        }

        private void Resize(int newWidth, int newHeight)
        {
            PixelWidth = newWidth;
            PixelHeight = newHeight;
            Magnification = Settings.Default.DefaultMagnification;
        }

        public void RefreshMeasureAndMagnification()
        {
            UpdateToolProperties();
            InvalidateMeasure();
            UpdateMagnification();
        }

        public void UpdateVisualRelated()
        {
            InvalidateVisual();
            RaiseImageUpdatedEvent?.Invoke(default, EventArgs.Empty);
            ChosenTool.SetActiveLayer(_activeLayer);
        }

        private void UpdateToolProperties()
        {
            ChosenTool?.SetDrawingCircumstances(Magnification, PixelWidth, PixelHeight, _activeLayer, _drawPreview);
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

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (IsMouseOver)
            {
                base.OnKeyDown(e);
                ChosenTool.OnKeyDown(e);
                RaiseImageUpdatedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var magnification = Magnification;
            var size = new Size(PixelWidth * magnification, PixelHeight * magnification);

            _vm?.MeasureAllLayer(size);

            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _vm?.ArrangeAllLayer(new Rect(finalSize));
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
            dc.DrawLine(pen, new Point(0, h * m), new Point(w * m, h * m));

            dc.DrawLine(pen, new Point(0, 0), new Point(0, h * m));
            dc.DrawLine(pen, new Point(w * m, 0), new Point(w * m, h * m));

            dc.Close();

            return dv;
        }

        public void UpdateMagnification()
        {
            RemoveVisualChild(_gridLines);
            _gridLines = CreateGridLines();

            RemoveVisualChild(_borderLine);
            _borderLine = CreateBorderLines();

            RemoveVisualChild(_drawPreview);
            _drawPreview = new DrawingLayer(this, "DrawPreview");

            AddVisualChild(_drawPreview);
            AddVisualChild(_borderLine);
            AddVisualChild(_gridLines);

            _vm?.InvalidateAllLayerVisual();

            RenderTransformOrigin = new Point(ActualWidth, ActualHeight);
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            UpdateToolProperties();

            InvalidateVisual();
        }

        public void UpdateMagnification(int zoom)
        {
            Magnification = zoom;

            RemoveVisualChild(_gridLines);
            _gridLines = CreateGridLines();

            RemoveVisualChild(_borderLine);
            _borderLine = CreateBorderLines();

            RemoveVisualChild(_drawPreview);
            _drawPreview = new DrawingLayer(this, "DrawPreview");

            AddVisualChild(_drawPreview);
            AddVisualChild(_borderLine);
            AddVisualChild(_gridLines);

            _vm.InvalidateAllLayerVisual();

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

        public List<LayerModel> GetLayerModels() => _vm.GetLayerModels();

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}