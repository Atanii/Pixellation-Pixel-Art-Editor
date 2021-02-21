using Pixellation.Components.Tools;
using Pixellation.Models;
using Pixellation.Properties;
using Pixellation.Utils.MementoPattern;
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
    /// <summary>
    /// Class representing the edited image and the corresponding framework element.
    /// </summary>
    public partial class PixelEditor : FrameworkElement, IPreviewable, INotifyPropertyChanged
    {
        #region PrivateFields
        private DrawingLayer _activeLayer;
        private Visual _gridLines;
        private Visual _borderLine;
        private DrawingLayer _drawPreview;
        private readonly Caretaker<IEditorEventType> _mementoCaretaker = Caretaker<IEditorEventType>.GetInstance();
        #endregion PrivateFields

        #region Properties
        /// <summary>
        /// List of <see cref="DrawingLayer"/>s the edited image consists of.
        /// </summary>
        public List<DrawingLayer> Layers { get; private set; } = new List<DrawingLayer>();

        private int _pixelWidth;
        /// <summary>
        /// Current width of the edited image in pixels.
        /// </summary>
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
        /// <summary>
        /// Current height of the edited image in pixels.
        /// </summary>
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
        /// <summary>
        /// Image is zoomed in this times on the UI.
        /// </summary>
        public int Magnification
        {
            get { return _magnification; }
            set
            {
                _magnification = value;
                UpdateMagnification();
                OnPropertyChanged();
            }
        }

        public int VisualCount { get; set; } = 0;
        #endregion Properties

        #region DependencyProperties
        /// <summary>
        /// Opacity of tiles in tiled-mode.
        /// </summary>
        public float TiledOpacity
        {
            get { return (float)GetValue(TiledOpacityProperty); }
            set { SetValue(TiledOpacityProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty TiledOpacityProperty =
         DependencyProperty.Register("TiledOpacity", typeof(float), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultTiledOpacity, (d, e) => { EditorSettingsChangeEvent?.Invoke(default, EventArgs.Empty); }
        ));

        /// <summary>
        /// Is tiled mode on or not.
        /// </summary>
        public bool TiledModeOn
        {
            get { return (bool)GetValue(TiledModeOnProperty); }
            set { SetValue(TiledModeOnProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty TiledModeOnProperty =
         DependencyProperty.Register("TiledModeOn", typeof(bool), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultTiledModeOn, (d, e) => { EditorSettingsChangeEvent?.Invoke(default, EventArgs.Empty); }
        ));

        /// <summary>
        /// Primary (left-click) color used in drawing.
        /// </summary>
        public System.Drawing.Color PrimaryColor
        {
            get { return (System.Drawing.Color)GetValue(PrimaryColorProperty); }
            set { SetValue(PrimaryColorProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty PrimaryColorProperty =
         DependencyProperty.Register("PrimaryColor", typeof(System.Drawing.Color), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultPrimaryColor
        ));

        /// <summary>
        /// Secondary (right-click) color used in drawing.
        /// </summary>
        public System.Drawing.Color SecondaryColor
        {
            get { return (System.Drawing.Color)GetValue(SecondaryColorProperty); }
            set { SetValue(SecondaryColorProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty SecondaryColorProperty =
         DependencyProperty.Register("SecondaryColor", typeof(System.Drawing.Color), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultSecondaryColor
        ));

        /// <summary>
        /// Using chosen tool as eraser (using transparent as primary color).
        /// </summary>
        public bool EraserModeOn
        {
            get { return (bool)GetValue(EraserModeOnProperty); }
            set { SetValue(EraserModeOnProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty EraserModeOnProperty =
         DependencyProperty.Register("EraserModeOn", typeof(bool), typeof(PixelEditor), new FrameworkPropertyMetadata(
            false
        ));

        /// <summary>
        /// Shows border around edited image.
        /// </summary>
        public bool ShowBorder
        {
            get { return (bool)GetValue(ShowBorderProperty); }
            set { SetValue(ShowBorderProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty ShowBorderProperty =
         DependencyProperty.Register("ShowBorder", typeof(bool), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultShowBorder, (d, e) => { EditorSettingsChangeEvent?.Invoke(default, EventArgs.Empty); }
        ));

        /// <summary>
        /// Shows grid on canvas.
        /// </summary>
        public bool ShowGrid
        {
            get { return (bool)GetValue(ShowGridProperty); }
            set { SetValue(ShowGridProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty ShowGridProperty =
         DependencyProperty.Register("ShowGrid", typeof(bool), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultShowGrid, (d, e) => { EditorSettingsChangeEvent?.Invoke(default, EventArgs.Empty); }
        ));

        /// <summary>
        /// Current drawing tool.
        /// </summary>
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
        /// <summary>
        /// Event for changing tool.
        /// </summary>
        private static event EventHandler RaiseToolChangeEvent;

        /// <summary>
        /// Event for changing editor settings (eg. toggle <see cref="ShowBorder"/>)
        /// </summary>
        private static event EventHandler EditorSettingsChangeEvent;

        /// <summary>
        /// Event for signaling change in the edited image.
        /// </summary>
        public event EventHandler RaiseImageUpdatedEvent;

        /// <summary>
        /// Event used for one- and twoway databinding.
        /// Marks change regarding one of the properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public event LayerListEventHandler LayerListChanged;
        #endregion Event

        /// <summary>
        /// Binds eventhandlers and initializes an editable image with the default size, magnification and layer settings.
        /// </summary>
        public PixelEditor()
        {
            Cursor = Cursors.Pen;

            PixelWidth = Settings.Default.DefaultImageSize;
            PixelHeight = Settings.Default.DefaultImageSize;
            Magnification = Settings.Default.DefaultMagnification;

            Layers.Add(new DrawingLayer(this, "default"));

            BaseTool.RaiseToolEvent += HandleToolEvent;
            RaiseToolChangeEvent += (d, e) => { UpdateToolProperties(); };
            EditorSettingsChangeEvent += (d, e) => { SetOrRefreshMeasureVisualsMagnification(); };

            Init();
        }

        #region Init And New Image
        /// <summary>
        /// Starts new image (dropping the previous one if there was).
        /// <see cref="Magnification"/> will be reset to the default value.
        /// </summary>
        /// <param name="pixelWidth">New <see cref="PixelWidth"/>.</param>
        /// <param name="pixelHeight">New <see cref="PixelHeight"/>.</param>
        public void NewImage(int pixelWidth = 32, int pixelHeight = 32)
        {
            DeleteAllVisualChildren();

            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            Magnification = Settings.Default.DefaultMagnification;

            Layers.Add(new DrawingLayer(this, "default"));

            Init();
        }

        /// <summary>
        /// Starts new image (dropping the previous one if there was).
        /// <see cref="Magnification"/> will be reset to the default value.
        /// </summary>
        /// <param name="imageToEdit">Image to edit.</param>
        public void NewImage(WriteableBitmap imageToEdit)
        {
            DeleteAllVisualChildren();

            PixelWidth = imageToEdit.PixelWidth;
            PixelHeight = imageToEdit.PixelHeight;
            Magnification = Settings.Default.DefaultMagnification;

            if (imageToEdit == null)
            {
                Layers.Add(new DrawingLayer(this, "default"));
            }
            else
            {
                Layers.Add(new DrawingLayer(this, imageToEdit, "default"));
            }

            Init();
        }

        /// <summary>
        /// Starts new image (dropping the previous one if there was).
        /// <see cref="Magnification"/> will be reset to the default value.
        /// </summary>
        /// <param name="models"><see cref="List<LayerModel>"/> containing the layers for edit.</param>
        /// <param name="pixelWidth">New <see cref="PixelWidth"/>.</param>
        /// <param name="pixelHeight">New <see cref="PixelHeight"/>.</param>
        public void NewImage(List<LayerModel> models, int pixelWidth = 32, int pixelHeight = 32)
        {
            DeleteAllVisualChildren();

            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            Magnification = Settings.Default.DefaultMagnification;

            foreach (var model in models)
            {
                Layers.Add(new DrawingLayer(
                    this,
                    model
                ));
            }

            Init();
        }

        /// <summary>
        /// Sets default actively edited layer and refresh.
        /// </summary>
        private void Init()
        {
            if (Layers.Count > 0)
            {
                _activeLayer = Layers[0];
            }

            RefreshVisualsThenSignalUpdate();
        }
        #endregion Init And New Image

        #region Update, refresh...
        /// <summary>
        /// Refresh visuals, magnification related properties.
        /// </summary>
        public void SetOrRefreshMeasureVisualsMagnification()
        {
            // Setting up layers and helper visuals
            ResetLayerAndHelperVisuals();
            // Refresh canvas position
            RefreshPositionAndAlignment();
            // Setting up tools
            UpdateToolProperties();
            // Refresh size and visuals
            InvalidateMeasure();
            InvalidateVisual();
        }

        /// <summary>
        /// Refresh visuals (excluding editable layers), magnification related properties.
        /// </summary>
        public void UpdateMagnification()
        {
            // Reset helpers
            ResetHelperVisuals();
            // Refresh canvas position
            RefreshPositionAndAlignment();
            // Setting up tools
            UpdateToolProperties();
            // Refresh size and visuals
            InvalidateMeasure();
            InvalidateVisual();
        }

        /// <summary>
        /// Refresh position and alignment, origin of <see cref="PixelEditor"/> on the UI.
        /// </summary>
        public void RefreshPositionAndAlignment()
        {
            RenderTransformOrigin = new Point(ActualWidth, ActualHeight);
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
        }

        /// <summary>
        /// Refresh then signal changes with <see cref="RaiseImageUpdatedEvent"/>.
        /// </summary>
        public void RefreshVisualsThenSignalUpdate()
        {
            SetOrRefreshMeasureVisualsMagnification();
            RaiseImageUpdatedEvent?.Invoke(default, EventArgs.Empty);
        }

        /// <summary>
        /// Resets properties in <see cref="ChosenTool"/> like <see cref="Magnification"/> or <see cref="PixelWidth"/>.
        /// </summary>
        private void UpdateToolProperties()
        {
            ChosenTool?.SetDrawingCircumstances(Magnification, PixelWidth, PixelHeight, _activeLayer, _drawPreview);
        }
        #endregion Update, refresh...

        #region Misc overrides
        /// <summary>
        /// Overrides <see cref="MeasureOverride(Size)"/> to take <see cref="Magnification"/> and layers into account.
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var size = new Size(PixelWidth * Magnification, PixelHeight * Magnification);
            MeasureAllLayer(size);
            return size;
        }

        /// <summary>
        /// Overrides <see cref="ArrangeOverride(Size)"/> to take layers into account.
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeAllLayer(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Overrides <see cref="VisualChildrenCount"/> to return <see cref="VisualCount"/>.
        /// </summary>
        protected override int VisualChildrenCount => VisualCount;

        /// <summary>
        /// Gives the child <see cref="Visual"/> based on the index.
        /// </summary>
        /// <param name="index">Index of the descendant <see cref="Visual"/></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            // Layers are on the bottom of the stack.
            if (index < Layers.Count)
            {
                return Layers[(Layers.Count - 1) - index];
            }
            // Preview is located bet
            else if (index == VisualCount - 3)
            {
                return _drawPreview;
            }
            else if (index == VisualCount - 2)
            {
                if (_gridLines != null)
                {
                    return _gridLines;
                }
                else
                {
                    return _borderLine;
                }
            }
            else if (index == VisualCount - 1)
            {
                if (_borderLine != null)
                {
                    return _borderLine;
                }
                else
                {
                    return _gridLines;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Handle <see cref="Visual"/> children being added or removed.
        /// </summary>
        /// <param name="visualAdded"><see cref="Visual"/> child added.</param>
        /// <param name="visualRemoved"><see cref="Visual"/> child removed.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            // Track when objects are added and removed
            if (visualAdded != null)
            {
                ++VisualCount;
            }
            if (visualRemoved != null)
            {
                --VisualCount;
            }

            // Call base function
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
        #endregion Misc overrides

        #region Create visuals
        /// <summary>
        /// Creates gridlines <see cref="Visual"/> for showing grid on the canvas.
        /// </summary>
        /// <returns>Gridlines <see cref="Visual"/>.</returns>
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

        /// <summary>
        /// Creates borderlines <see cref="Visual"/> for showing grid on the canvas.
        /// </summary>
        /// <returns>Borderlines <see cref="Visual"/>.</returns>
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

        #region Handlers for custom events
        /// <summary>
        /// Handles events regarding drawing tools.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Routes <see cref="OnMouseMove(MouseEventArgs)"/> to <see cref="ChosenTool"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsMouseCaptured)
                ChosenTool.OnMouseMove(e);
        }

        /// <summary>
        /// Routes <see cref="OnMouseLeftButtonDown(MouseButtonEventArgs)"/> to <see cref="ChosenTool"/>.
        /// Based on <see cref="EraserModeOn"/> it sets <see cref="System.Drawing.Color.Transparent"/> or <see cref="PrimaryColor"/> as drawing color.
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// Routes <see cref="OnMouseRightButtonDown(MouseButtonEventArgs)"/> to <see cref="ChosenTool"/>.
        /// It sets <see cref="SecondaryColor"/> as drawing color.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            CaptureMouse();

            ChosenTool.SetDrawColor(SecondaryColor);
            ChosenTool.OnMouseDown(e);
        }

        /// <summary>
        /// Routes <see cref="OnMouseLeftButtonUp(MouseButtonEventArgs)"/> to <see cref="ChosenTool"/>.
        /// Signals an <see cref="RaiseImageUpdatedEvent"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
            ChosenTool.OnMouseUp(e);
            RaiseImageUpdatedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Routes <see cref="OnMouseRightButtonUp(MouseButtonEventArgs)"/> to <see cref="ChosenTool"/>.
        /// Signals an <see cref="RaiseImageUpdatedEvent"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
            ChosenTool.OnMouseUp(e);
            RaiseImageUpdatedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Routes <see cref="OnKeyDown(object, KeyEventArgs)"/> to <see cref="ChosenTool"/>.
        /// Signals an <see cref="RaiseImageUpdatedEvent"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (IsMouseOver)
            {
                base.OnKeyDown(e);
                ChosenTool.OnKeyDown(e);
                RaiseImageUpdatedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Used as change notification for one- and twoway binding with <see cref="DependencyProperty"/> objects.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion On... Event handler functions
    }
}