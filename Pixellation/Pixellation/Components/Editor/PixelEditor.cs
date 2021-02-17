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

        public System.Drawing.Color PrimaryColor
        {
            get { return (System.Drawing.Color)GetValue(PrimaryColorProperty); }
            set { SetValue(PrimaryColorProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty PrimaryColorProperty =
         DependencyProperty.Register("PrimaryColor", typeof(System.Drawing.Color), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultPrimaryColor
        ));

        public System.Drawing.Color SecondaryColor
        {
            get { return (System.Drawing.Color)GetValue(SecondaryColorProperty); }
            set { SetValue(SecondaryColorProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty SecondaryColorProperty =
         DependencyProperty.Register("SecondaryColor", typeof(System.Drawing.Color), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultSecondaryColor
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

        public VisualManager VisualAndLayerManager { get; private set; }

        public PixelEditor()
        {
            Magnification = Settings.Default.DefaultMagnification;
            VisualAndLayerManager = new VisualManager(this);
            Init(Settings.Default.DefaultImageSize, Settings.Default.DefaultImageSize);
            RefreshMeasureAndMagnification();
        }

        public PixelEditor(int width, int height, int defaultMagnification)
        {
            Magnification = defaultMagnification;
            VisualAndLayerManager = new VisualManager(this);
            Init(width, height);
            RefreshMeasureAndMagnification();
        }

        private void Init(int width, int height)
        {
            PixelWidth = width;
            PixelHeight = height;

            var layers = new List<DrawingLayer>
            {
                new DrawingLayer(this, "default")
            };

            _gridLines = CreateGridLines();
            _borderLine = CreateBorderLines();
            _drawPreview = new DrawingLayer(this, "DrawPreview");

            Cursor = Cursors.Pen;

            BaseTool.RaiseToolEvent += HandleToolEvent;
            RaiseToolChangeEvent += (d, e) => { UpdateToolProperties(); };

            VisualAndLayerManager.SetVisuals(layers, _gridLines, _borderLine, _drawPreview);

            VisualAndLayerManager.VisualsChanged += (a, b) => { UpdateVisualRelated(); };
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

            VisualAndLayerManager.SetVisuals(layers, _gridLines, _borderLine, _drawPreview);

            VisualAndLayerManager.VisualsChanged += (a, b) => { UpdateVisualRelated(); };
        }

        private void Init(List<DrawingLayer> layers)
        {
            _gridLines = CreateGridLines();
            _borderLine = CreateBorderLines();
            _drawPreview = new DrawingLayer(this, "DrawPreview");

            Cursor = Cursors.Pen;

            BaseTool.RaiseToolEvent += HandleToolEvent;
            RaiseToolChangeEvent += (d, e) => { UpdateToolProperties(); };

            VisualAndLayerManager.SetVisuals(layers, _gridLines, _borderLine, _drawPreview);

            VisualAndLayerManager.VisualsChanged += (a, b) => { UpdateVisualRelated(); };
        }

        public void ToggleTiled()
        {
            Tiled = !Tiled;
            VisualAndLayerManager.InvalidateAllLayerVisual();
        }

        public void SetTiledOpacity(float newOpacity)
        {
            TiledOpacity = newOpacity;
            VisualAndLayerManager.InvalidateAllLayerVisual();
        }

        public void NewImage(int width = 32, int height = 32)
        {
            PixelWidth = width;
            PixelHeight = height;

            VisualAndLayerManager.DeleteAllVisualChildren();

            Init();

            RefreshMeasureAndMagnification();
        }

        public void NewImage(WriteableBitmap imageToEdit)
        {
            PixelWidth = imageToEdit.PixelWidth;
            PixelHeight = imageToEdit.PixelHeight;

            VisualAndLayerManager.DeleteAllVisualChildren();

            Init(imageToEdit);

            UpdateToolProperties();

            InvalidateMeasure();
            InvalidateVisual();
        }

        public void NewImage(List<LayerModel> models, int width = 32, int height = 32)
        {
            PixelWidth = width;
            PixelHeight = height;

            VisualAndLayerManager.DeleteAllVisualChildren();

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
                    PrimaryColor = (System.Drawing.Color)e.Value;
                    break;

                case ToolEventType.NOTHING:
                default:
                    break;
            }
        }

        protected override int VisualChildrenCount => VisualAndLayerManager.VisualCount;

        protected override Visual GetVisualChild(int index) => VisualAndLayerManager.GetVisualChild(index);

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

            ChosenTool.SetDrawColor(PrimaryColor);
            ChosenTool.OnMouseDown(e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            CaptureMouse();

            ChosenTool.SetDrawColor(SecondaryColor);
            ChosenTool.OnMouseDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
            ChosenTool.OnMouseUp(e);
            RaiseImageUpdatedEvent?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
            ChosenTool.OnMouseUp(e);
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

            VisualAndLayerManager?.MeasureAllLayer(size);

            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            VisualAndLayerManager?.ArrangeAllLayer(new Rect(finalSize));
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

            VisualAndLayerManager?.InvalidateAllLayerVisual();

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

            VisualAndLayerManager.InvalidateAllLayerVisual();

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

        public ImageSource GetImageSource() => VisualAndLayerManager.GetAllMergedImageSource();

        public List<LayerModel> GetLayerModels() => VisualAndLayerManager.GetLayerModels();

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}