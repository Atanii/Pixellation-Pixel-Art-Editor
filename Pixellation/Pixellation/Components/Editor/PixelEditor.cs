using Pixellation.Components.Editor.Event;
using Pixellation.Interfaces;
using Pixellation.Models;
using Pixellation.Properties;
using Pixellation.Tools;
using Pixellation.Utils;
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
    public partial class PixelEditor : FrameworkElement, INotifyPropertyChanged, IDrawingHelper, IFrameProvider
    {
        #region PrivateFields

        private const string FramesCaretakerKey = "FramesCaretaker";
        public const string DefaultLayerName = "default";

        private readonly PixellationCaretakerManager _caretaker = PixellationCaretakerManager.GetInstance();

        private Visual _gridLines;
        private Visual _borderLine;

        private DrawingLayer _drawPreview;

        #endregion PrivateFields

        #region Properties

        private DrawingLayer _onionLayer;

        /// <summary>
        /// Layer for showing the previous frame with low opacity.
        /// </summary>
        private DrawingLayer OnionLayer
        {
            get => _onionLayer;
            set
            {
                _onionLayer = value;
                RefreshOnionLayer();
            }
        }

        private bool _onionModeEnabled;
        
        /// <summary>
        /// Indicates if onion mode is enabled in the editor.
        /// </summary>
        public bool OnionModeEnabled
        {
            get => _onionModeEnabled;
            set
            {
                _onionModeEnabled = value;
                RefreshOnionLayer();
            }
        }

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

        /// <summary>
        /// <see cref="PixelWidth"/> with applied <see cref="Magnification"/>.
        /// </summary>
        public int MagnifiedWidth => PixelWidth * Magnification;

        /// <summary>
        /// <see cref="PixelHeight"/> with applied <see cref="Magnification"/>.
        /// </summary>
        public int MagnifiedHeight => PixelHeight * Magnification;

        /// <summary>
        /// Count of visualchildren.
        /// </summary>
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
            Settings.Default.DefaultTiledOpacity, (d, e) => { VisualHelperSettingsChanged?.Invoke(); }
        ));

        /// <summary>
        /// Is tiled mode on or not.
        /// </summary>
        public bool TiledModeEnabled
        {
            get { return (bool)GetValue(TiledModeEnabledProperty); }
            set { SetValue(TiledModeEnabledProperty, value); OnPropertyChanged(); }
        }

        public static readonly DependencyProperty TiledModeEnabledProperty =
         DependencyProperty.Register("TiledModeEnabled", typeof(bool), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultTiledModeEnabled, (d, e) => { VisualHelperSettingsChanged?.Invoke(); }
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
        /// Shows border around edited image.
        /// </summary>
        public bool ShowBorder
        {
            get { return (bool)GetValue(ShowBorderProperty); }
            set { SetValue(ShowBorderProperty, value); OnPropertyChanged(); }
        }

        public static readonly DependencyProperty ShowBorderProperty =
         DependencyProperty.Register("ShowBorder", typeof(bool), typeof(PixelEditor), new FrameworkPropertyMetadata(
            Settings.Default.DefaultShowBorder, (d, e) => { VisualHelperSettingsChanged?.Invoke(); }
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
            Settings.Default.DefaultShowGrid, (d, e) => { VisualHelperSettingsChanged?.Invoke(); }
        ));

        /// <summary>
        /// Current drawing tool.
        /// </summary>
        public ITool ChosenTool
        {
            get { return (ITool)GetValue(ChosenToolProperty); }
            set
            {
                SetValue(ChosenToolProperty, value);
                Cursor = ChosenTool.ToolCursor;
            }
        }

        public readonly DependencyProperty ChosenToolProperty =
         DependencyProperty.Register("ChosenTool", typeof(ITool), typeof(PixelEditor), new PropertyMetadata(
             null, (d, e) => { RaiseToolChangeEvent?.Invoke(); }
        ));

        #endregion DependencyProperties

        #region Event

        /// <summary>
        /// Event for changing tool.
        /// </summary>
        private static event PixelEditorEventHandler RaiseToolChangeEvent;

        /// <summary>
        /// Event for changing editor settings (eg. toggle <see cref="ShowBorder"/>)
        /// </summary>
        private static event PixelEditorEventHandler VisualHelperSettingsChanged;

        /// <summary>
        /// Event for signaling change in the edited image.
        /// </summary>
        public static event PixelEditorEventHandler RaiseImageUpdatedEvent;

        /// <summary>
        /// Event used for one- and twoway databinding.
        /// Marks change regarding one of the properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Event

        /// <summary>
        /// Default and only constructor. Binds eventhandlers and initializes an editable image with the default size, magnification and layer settings.
        /// </summary>

        public PixelEditor()
        {
            // Default cursor is a pen.
            Cursor = Cursors.Pen;

            _caretaker.InitCaretaker(FramesCaretakerKey, autoActivateIfInitial: false);

            AddDrawingFrame(0, DefaultLayerName);

            PixelWidth = Settings.Default.DefaultImageSize;
            PixelHeight = Settings.Default.DefaultImageSize;
            Magnification = Settings.Default.DefaultMagnification;

            BaseTool.RaiseToolEvent += HandleToolEvent;
            RaiseToolChangeEvent += UpdateToolProperties;
            VisualHelperSettingsChanged += VisualHelperRefresh;

            AddLayer(new DrawingLayer(this, DefaultLayerName));

            SetActiveLayer();
        }

        #region Init And New Image

        /// <summary>
        /// Starts new project (dropping the previous one if there was).
        /// <see cref="Magnification"/> will be reset to the default value.
        /// </summary>
        /// <param name="pixelWidth">New <see cref="PixelWidth"/>.</param>
        /// <param name="pixelHeight">New <see cref="PixelHeight"/>.</param>
        public void NewProject(int pixelWidth = 32, int pixelHeight = 32)
        {
            DeleteAllVisualChildren();

            ResetVisualHelperSettings();

            _caretaker.InitCaretaker(FramesCaretakerKey, autoActivateIfInitial: false);

            AddDrawingFrame(0, "Default");

            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            Magnification = Settings.Default.DefaultMagnification;

            AddLayer(new DrawingLayer(this, DefaultLayerName));

            SetActiveLayer();
        }

        /// <summary>
        /// Starts new project (dropping the previous one if there was).
        /// <see cref="Magnification"/> will be reset to the default value.
        /// </summary>
        /// <param name="imageToEdit">Image to edit.</param>
        public void NewProject(WriteableBitmap imageToEdit)
        {
            DeleteAllVisualChildren();

            ResetVisualHelperSettings();

            _caretaker.InitCaretaker(FramesCaretakerKey, autoActivateIfInitial: false);

            AddDrawingFrame(0, "Default");

            PixelWidth = imageToEdit.PixelWidth;
            PixelHeight = imageToEdit.PixelHeight;
            Magnification = Settings.Default.DefaultMagnification;

            if (imageToEdit == null)
            {
                AddLayer(new DrawingLayer(this, DefaultLayerName));
            }
            else
            {
                AddLayer(new DrawingLayer(this, imageToEdit, DefaultLayerName));
            }

            SetActiveLayer();
        }

        /// <summary>
        /// Starts new image (dropping the previous one if there was).
        /// <see cref="Magnification"/> will be reset to the default value.
        /// </summary>
        /// <param name="models"><see cref="List<LayerModel>"/> containing the layers for edit.</param>
        /// <param name="pixelWidth">New <see cref="PixelWidth"/>.</param>
        /// <param name="pixelHeight">New <see cref="PixelHeight"/>.</param>
        public void NewProject(List<DrawingFrame> frames)
        {
            DeleteAllVisualChildren();

            ResetVisualHelperSettings();

            _caretaker.InitCaretaker(FramesCaretakerKey, autoActivateIfInitial: false);

            PixelWidth = frames[0].Layers[0].Bitmap.PixelWidth;
            PixelHeight = frames[0].Layers[0].Bitmap.PixelHeight;

            AddDrawingFrames(frames);

            Magnification = Settings.Default.DefaultMagnification;

            SetActiveLayer();
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
        /// Reset the settings of visualhelpers to default.
        /// </summary>
        private void ResetVisualHelperSettings()
        {
            TiledModeEnabled = Settings.Default.DefaultTiledModeEnabled;
            TiledOpacity = Settings.Default.DefaultTiledOpacity;
            ShowBorder = Settings.Default.DefaultShowBorder;
            ShowGrid = Settings.Default.DefaultShowGrid;
        }

        /// <summary>
        /// Refresh visualhelpers.
        /// </summary>
        private void VisualHelperRefresh()
        {
            ResetHelperVisuals();
            ActiveFrame.SetTiledMode(TiledModeEnabled);
            ActiveFrame.SetTiledModeOpacity(TiledOpacity);
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
            ChosenTool?.SetMagnification(Magnification);
            // Refresh size and visuals
            InvalidateMeasure();
            InvalidateVisual();
        }

        /// <summary>
        /// Refreshes onionlayer.
        /// </summary>
        private void RefreshOnionLayer()
        {
            if (_onionModeEnabled && ActiveFrameIndex > 0 && _onionLayer != null)
            {
                _onionLayer.Bitmap = MergeUtils.MergeAll(Frames[ActiveFrameIndex - 1].Layers);
            }
            if (!_onionModeEnabled && _onionLayer != null)
            {
                _onionLayer.Clear();
            }
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
            RaiseImageUpdatedEvent?.Invoke();
        }

        /// <summary>
        /// Resets properties in <see cref="ChosenTool"/> like <see cref="Magnification"/> or <see cref="PixelWidth"/> .
        /// </summary>
        private void UpdateToolProperties()
        {
            Cursor = ChosenTool?.ToolCursor ?? Cursors.Pen;
            ChosenTool?.SetAllDrawingCircumstances(Magnification, PixelWidth, PixelHeight, ActiveLayer, _drawPreview);
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
            else if (index == VisualCount - 4)
            {
                return _onionLayer;
            }
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

        #region Misc

        /// <summary>
        /// Checks if the given point is inside the bounds of the edited image.
        /// </summary>
        /// <param name="p">Point to check.</param>
        /// <returns>True if point is contained, false otherwise.</returns>
        public bool InBounds(IntPoint p) => p.X >= 0 && p.Y >= 0 && p.X < PixelWidth && p.Y < PixelHeight;

        #endregion Misc

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
        private void HandleToolEvent(ToolEventArgs e)
        {
            switch (e.Type)
            {
                case ToolEventType.PRIMARYCOLOR:
                    PrimaryColor = e.Value;
                    break;

                case ToolEventType.SECONDARY:
                    SecondaryColor = e.Value;
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

            if (!ActiveLayer.Visible || !ActiveFrame.Visible)
            {
                return;
            }

            // Making sure mouse is captured only(!) within the bounds of the drawing area.
            // Also clearing preview assuring that no pointer or other preview graphics got stucked when leaving the drawing area.
            var p = e.GetPosition(this).DivideByIntAsIntPoint(_magnification);
            if (InBounds(p))
            {
                CaptureMouse();
            }
            else if (e.LeftButton != MouseButtonState.Pressed && e.RightButton != MouseButtonState.Pressed)
            {
                ChosenTool.OnMouseUp(e);
                ReleaseMouseCapture();
                RaiseImageUpdatedEvent?.Invoke();
                ActiveLayer.InvalidateVisual();
            }

            if (IsMouseCaptured)
            {
                ChosenTool.OnMouseMoveTraceWithPointer(e);
                ChosenTool.OnMouseMove(e);
                RaiseImageUpdatedEvent?.Invoke();
                ActiveLayer.InvalidateVisual();
            }
        }

        /// <summary>
        /// Routes <see cref="OnMouseLeftButtonDown(MouseButtonEventArgs)"/> to <see cref="ChosenTool"/>.
        /// Based on <see cref="EraserModeOn"/> it sets <see cref="System.Drawing.Color.Transparent"/> or <see cref="PrimaryColor"/> as drawing color.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!ActiveLayer.Visible || !ActiveFrame.Visible)
            {
                return;
            }

            if (IsMouseCaptured)
            {
                ChosenTool.SetDrawColor(PrimaryColor);
                ChosenTool.OnMouseDown(e);
                RaiseImageUpdatedEvent?.Invoke();
                ActiveLayer.InvalidateVisual();
            }
        }

        /// <summary>
        /// Routes <see cref="OnMouseRightButtonDown(MouseButtonEventArgs)"/> to <see cref="ChosenTool"/>.
        /// It sets <see cref="SecondaryColor"/> as drawing color.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            if (!ActiveLayer.Visible || !ActiveFrame.Visible)
            {
                return;
            }

            if (IsMouseCaptured)
            {
                ChosenTool.SetDrawColor(SecondaryColor);
                ChosenTool.OnMouseDown(e);
                RaiseImageUpdatedEvent?.Invoke();
                ActiveLayer.InvalidateVisual();
            }
        }

        /// <summary>
        /// Routes <see cref="OnMouseLeftButtonUp(MouseButtonEventArgs)"/> to <see cref="ChosenTool"/>.
        /// Signals an <see cref="RaiseImageUpdatedEvent"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!ActiveLayer.Visible || !ActiveFrame.Visible)
            {
                return;
            }

            ChosenTool.OnMouseUp(e);
            RaiseImageUpdatedEvent?.Invoke();
            ActiveLayer.InvalidateVisual();

            var p = e.GetPosition(this).DivideByIntAsIntPoint(_magnification);
            if (!InBounds(p))
            {
                ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Routes <see cref="OnMouseRightButtonUp(MouseButtonEventArgs)"/> to <see cref="ChosenTool"/>.
        /// Signals an <see cref="RaiseImageUpdatedEvent"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!ActiveLayer.Visible || !ActiveFrame.Visible)
            {
                return;
            }

            ChosenTool.OnMouseUp(e);
            RaiseImageUpdatedEvent?.Invoke();
            ActiveLayer.InvalidateVisual();

            var p = e.GetPosition(this).DivideByIntAsIntPoint(_magnification);
            if (!InBounds(p))
            {
                ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Signals to <see cref="PixelEditor"/> if the mouse left the containerelement.
        /// Since this class uses an overriden measure, a signal from the container element is needed in case of enlarged, scolled images.
        /// </summary>
        /// <param name="left">True if mouse left the area.</param>
        /// <param name="e"></param>
        public void MouseLeftContainerElement(bool left, MouseEventArgs e)
        {
            if (left && IsMouseCaptured)
            {
                ChosenTool.OnMouseUp(e);
                ReleaseMouseCapture();
                RaiseImageUpdatedEvent?.Invoke();
                ActiveLayer.InvalidateVisual();
            }
        }

        /// <summary>
        /// Routes <see cref="OnKeyDown(object, KeyEventArgs)"/> to <see cref="ChosenTool"/>.
        /// Signals an <see cref="RaiseImageUpdatedEvent"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + C, Ctrl + V, Ctlr + X are only applied when the drawing area is focused with mouseposition.
            if (IsMouseCaptured)
            {
                base.OnKeyDown(e);
                ChosenTool.OnKeyDown(e);
                RaiseImageUpdatedEvent?.Invoke();
                ActiveLayer.InvalidateVisual();
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