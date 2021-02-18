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
        #region PrivateFields
        private DrawingLayer _activeLayer;
        private Visual _gridLines;
        private Visual _borderLine;
        private DrawingLayer _drawPreview;
        #endregion PrivateFields

        #region Properties
        public List<DrawingLayer> Layers { get; private set; }
        public VisualManager VisualAndLayerManager { get; private set; }

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
                RefreshMeasureVisualsMagnification();
                OnPropertyChanged();
            }
        }
        #endregion Properties

        #region DependencyProperties
        public float TiledOpacity
        {
            get { return (float)GetValue(TiledOpacityProperty); }
            set { SetValue(TiledOpacityProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty TiledOpacityProperty =
         DependencyProperty.Register("TiledOpacity", typeof(float), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultTiledOpacity, (d, e) => { EditorSettingsChangeEvent?.Invoke(default, EventArgs.Empty); }
        ));

        public bool TiledModeOn
        {
            get { return (bool)GetValue(TiledModeOnProperty); }
            set { SetValue(TiledModeOnProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty TiledModeOnProperty =
         DependencyProperty.Register("TiledModeOn", typeof(bool), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultTiledModeOn, (d, e) => { EditorSettingsChangeEvent?.Invoke(default, EventArgs.Empty); }
        ));

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

        public bool EraserModeOn
        {
            get { return (bool)GetValue(EraserModeOnProperty); }
            set { SetValue(EraserModeOnProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty EraserModeOnProperty =
         DependencyProperty.Register("EraserModeOn", typeof(bool), typeof(PixelEditor), new FrameworkPropertyMetadata(
            false
        ));

        public bool ShowBorder
        {
            get { return (bool)GetValue(ShowBorderProperty); }
            set { SetValue(ShowBorderProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty ShowBorderProperty =
         DependencyProperty.Register("ShowBorder", typeof(bool), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultShowBorder, (d, e) => { EditorSettingsChangeEvent?.Invoke(default, EventArgs.Empty); }
        ));

        public bool ShowGrid
        {
            get { return (bool)GetValue(ShowGridProperty); }
            set { SetValue(ShowGridProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty ShowGridProperty =
         DependencyProperty.Register("ShowGrid", typeof(bool), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultShowGrid, (d, e) => { EditorSettingsChangeEvent?.Invoke(default, EventArgs.Empty); }
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
        #endregion DependencyProperties

        #region Event
        private static event EventHandler RaiseToolChangeEvent;
        private static event EventHandler EditorSettingsChangeEvent;
        public event EventHandler RaiseImageUpdatedEvent;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Event

        public PixelEditor()
        {
            Cursor = Cursors.Pen;

            Magnification = Settings.Default.DefaultMagnification;
            PixelWidth = Settings.Default.DefaultImageSize;
            PixelHeight = Settings.Default.DefaultImageSize;

            var layers = new List<DrawingLayer>
            {
                new DrawingLayer(this, "default")
            };

            BaseTool.RaiseToolEvent += HandleToolEvent;
            RaiseToolChangeEvent += (d, e) => { UpdateToolProperties(); };
            EditorSettingsChangeEvent += (d, e) => { RefreshMeasureVisualsMagnification(); };

            Init(layers);
        }

        #region Init And New Image
        public void NewImage(int width = 32, int height = 32)
        {
            Magnification = Settings.Default.DefaultMagnification;
            PixelWidth = width;
            PixelHeight = height;

            VisualAndLayerManager.DeleteAllVisualChildren();

            var layers = new List<DrawingLayer>
            {
                new DrawingLayer(this, "default")
            };

            Init(layers);
        }

        public void NewImage(WriteableBitmap imageToEdit)
        {
            Magnification = Settings.Default.DefaultMagnification;
            PixelWidth = imageToEdit.PixelWidth;
            PixelHeight = imageToEdit.PixelHeight;

            VisualAndLayerManager.DeleteAllVisualChildren();

            var layers = new List<DrawingLayer>();
            if (imageToEdit == null)
            {
                layers.Add(new DrawingLayer(this, "default"));
            }
            else
            {
                layers.Add(new DrawingLayer(this, imageToEdit, "default"));
            }

            Init(layers);
        }

        public void NewImage(List<LayerModel> models, int width = 32, int height = 32)
        {
            Magnification = Settings.Default.DefaultMagnification;
            PixelWidth = width;
            PixelHeight = height;

            VisualAndLayerManager.DeleteAllVisualChildren();

            var layers = new List<DrawingLayer>();
            foreach (var model in models)
            {
                layers.Add(new DrawingLayer(
                    this,
                    model
                ));
            }

            Init(layers);
        }

        private void Init(List<DrawingLayer> layers)
        {
            _gridLines = CreateGridLines();
            _borderLine = CreateBorderLines();
            _drawPreview = new DrawingLayer(this, "DrawPreview");

            VisualAndLayerManager = new VisualManager(this);
            VisualAndLayerManager.SetVisuals(layers, _gridLines, _borderLine, _drawPreview);
            VisualAndLayerManager.VisualsChanged += (a, b) => { UpdateVisualRelated(); };

            RefreshMeasureVisualsMagnification();
        }
        #endregion Init And New Image

        #region Transforms
        private void Resize(int newWidth, int newHeight)
        {
            PixelWidth = newWidth;
            PixelHeight = newHeight;
            Magnification = Settings.Default.DefaultMagnification;
        }
        #endregion Transforms

        #region Update, refresh...
        public void RefreshMeasureVisualsMagnification()
        {
            UpdateToolProperties();
            RefreshHelperVisuals();
            RefreshPositionAndAlignment();
            InvalidateMeasure();
            InvalidateVisual();
            VisualAndLayerManager?.InvalidateAllLayerVisual();
        }

        public void RefreshHelperVisuals()
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
        }

        public void RefreshPositionAndAlignment()
        {
            RenderTransformOrigin = new Point(ActualWidth, ActualHeight);
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
        }

        public void UpdateVisualRelated()
        {
            RefreshMeasureVisualsMagnification();
            RaiseImageUpdatedEvent?.Invoke(default, EventArgs.Empty);
        }

        private void UpdateToolProperties()
        {
            ChosenTool?.SetDrawingCircumstances(Magnification, PixelWidth, PixelHeight, _activeLayer, _drawPreview);
        }
        #endregion Update, refresh...

        #region Misc overrides
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

        #endregion Misc overrides

        #region Create visuals
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

        #endregion Create visuals

        #region Getters
        protected override int VisualChildrenCount => VisualAndLayerManager.VisualCount;

        protected override Visual GetVisualChild(int index) => VisualAndLayerManager.GetVisualChild(index);

        public WriteableBitmap GetWriteableBitmap() => _activeLayer.GetWriteableBitmap();

        public ImageSource GetImageSource() => VisualAndLayerManager.GetAllMergedImageSource();

        public List<LayerModel> GetLayerModels() => VisualAndLayerManager.GetLayerModels();
        #endregion Getters

        #region Handlers for custom events
        private void HandleToolEvent(object sender, ToolEventArgs e)
        {
            switch (e.Type)
            {
                case ToolEventType.PRIMARYCOLOR:
                    PrimaryColor = (System.Drawing.Color)e.Value;
                    break;

                case ToolEventType.SECONDARY:
                    SecondaryColor = (System.Drawing.Color)e.Value;
                    break;

                case ToolEventType.NOTHING:
                default:
                    break;
            }
        }
        #endregion Handlers for custom events

        #region On... Event handler functions
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

            if (EraserModeOn)
            {
                ChosenTool.SetDrawColor(System.Drawing.Color.Transparent);
            }
            else
            {
                ChosenTool.SetDrawColor(PrimaryColor);
            }
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

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion On... Event handler functions
    }
}