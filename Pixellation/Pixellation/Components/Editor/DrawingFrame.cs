using Pixellation.Components.Dialogs.StringInputDialog;
using Pixellation.Components.Editor.Memento;
using Pixellation.Components.Event;
using Pixellation.Interfaces;
using Pixellation.MementoPattern;
using Pixellation.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    /// <summary>
    /// Class representing a set of drawinglayers.
    /// </summary>
    public class DrawingFrame : FrameworkElement, IOriginator<FrameMemento, IPixelEditorEventType>
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid FrameGuid { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// String form of <see cref="FrameGuid"/>.
        /// </summary>
        public string Id => FrameGuid.ToString();

        /// <summary>
        /// Brush for drawing background when rendered as an element.
        /// </summary>
        public static readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Color.FromArgb(25, 50, 50, 50));

        /// <summary>
        /// Pen for drawing background when rendered as an element.
        /// </summary>
        public static readonly Pen BackgroundPen = new Pen(BackgroundBrush, 1d);

        /// <summary>
        /// Brush for drawing border when rendered as an element.
        /// </summary>
        public static readonly SolidColorBrush BorderBrush = new SolidColorBrush(Color.FromArgb(255, 10, 10, 10));

        /// <summary>
        /// Pen for drawing border when rendered as an element.
        /// </summary>
        public static readonly Pen BorderPen = new Pen(BorderBrush, 1d);

        /// <summary>
        /// Color for drawing text when rendered as an element.
        /// </summary>
        public static readonly Color TextDrawColor = Color.FromArgb(255, 0, 0, 0);

        /// <summary>
        /// Layers of this frame.
        /// </summary>
        public List<DrawingLayer> Layers { get; private set; } = new List<DrawingLayer>() { };

        /// <summary>
        /// Owner editor of the frame.
        /// </summary>
        private readonly IDrawingHelper _owner;

        private string _name;

        /// <summary>
        /// Name of the frame shown on the UI.
        /// </summary>
        public string FrameName
        {
            get => _name;
            set
            {
                _name = value;
                PropertyUpdated?.Invoke();
            }
        }

        private bool _visible = true;

        /// <summary>
        /// Indicates if visible or hidden.
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                InvalidateVisual();
                PropertyUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Represents all the underlying layer bitmaps as one merged bitmap.
        /// </summary>
        public WriteableBitmap Bitmap
        {
            get => MergeUtils.MergeAll(Layers);
        }

        #region Events

        /// <summary>
        /// Indicates propertyupdate.
        /// </summary>
        public static event FrameEventHandler PropertyUpdated;

        #endregion Events

        /// <summary>
        /// Inits frame.
        /// </summary>
        /// <param name="layers"><see cref="Layers"/>.</param>
        /// <param name="name"><see cref="FrameName"/>.</param>
        /// <param name="owner">Owner editor.</param>
        /// <param name="visible"><see cref="Visible"/>.</param>
        /// <param name="id"><see cref="FrameGuid"/>.</param>
        public DrawingFrame(List<DrawingLayer> layers, string name, IDrawingHelper owner, bool visible = true, Guid? id = null) : base()
        {
            if (id != null)
            {
                FrameGuid = (Guid)id;
            }
            _owner = owner;
            Layers = layers;
            FrameName = name;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            PixelEditor.RaiseImageUpdatedEvent += InvalidateVisual;
            Visible = visible;
        }

        /// <summary>
        /// Inits frame.
        /// </summary>
        /// <param name="name"><see cref="FrameName"/>.</param>
        /// <param name="owner">Owner editor.</param>
        /// <param name="visible"><see cref="Visible"/>.</param>
        /// <param name="generateDefaultLayer">Generates a blank default layer into <see cref="Layers"/>.</param>
        public DrawingFrame(string name, IDrawingHelper owner, bool visible = true, bool generateDefaultLayer = false)
        {
            _owner = owner;
            FrameName = name;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            PixelEditor.RaiseImageUpdatedEvent += InvalidateVisual;
            Visible = visible;
            if (generateDefaultLayer)
            {
                Layers.Add(new DrawingLayer(_owner, "Default"));
            }
        }

        /// <summary>
        /// Draws solid colored background.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void DrawBackground(DrawingContext dc, double x, double y, double width, double height)
        {
            dc.DrawRectangle(
                BackgroundBrush, BackgroundPen,
                new Rect(
                    x, y, width, height
                )
            );
        }

        /// <summary>
        /// Draws border.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void DrawBorder(DrawingContext dc, double x, double y, double width, double height)
        {
            dc.DrawRectangle(
                null, BorderPen,
                new Rect(
                    x, y, width, height
                )
            );
        }

        /// <summary>
        /// Draws visible layers onto top of each other.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="opacity">If provided, this opacity will be used for all drawn layers.</param>
        /// <param name="layerIndex">If provided, only the layer of this index will be drawn.</param>
        /// <param name="baseWidth">Width of the drawing area.</param>
        /// <param name="baseHeight">Height of the drawing area.</param>
        private void DrawLayers(DrawingContext dc, float? opacity = null, int layerIndex = -1, double? baseWidth = null, double? baseHeight = null)
        {
            var elementWidth = baseWidth ?? Width;
            var elementHeight = baseHeight ?? Height;

            // Calculate size and position according to the current magnification.
            var magnifiedWidth = _owner.MagnifiedWidth;
            var magnifiedHeight = _owner.MagnifiedHeight;

            double w, h;
            if (magnifiedWidth >= magnifiedHeight && magnifiedWidth > elementWidth)
            {
                w = elementWidth;
                h = (w / magnifiedWidth) * magnifiedHeight;
            }
            else if (magnifiedHeight >= magnifiedWidth && magnifiedHeight > elementHeight)
            {
                h = elementHeight;
                w = (h / magnifiedHeight) * magnifiedWidth;
            }
            else
            {
                w = magnifiedWidth;
                h = magnifiedHeight;
            }
            double x = (elementWidth - w) / 2;
            double y = (elementHeight - h) / 2;

            // Render layers.
            if (layerIndex != -1)
            {
                var l = Layers[layerIndex];
                if (!l.Visible)
                {
                    return;
                }
                var bmp = l.Bitmap.Clone();
                if (opacity != null)
                {
                    bmp.BlitRender(l.Bitmap, false, (float)opacity);
                }
                else
                {
                    bmp.BlitRender(l.Bitmap, false, (float)l.Opacity);
                }
                dc.DrawImage(bmp, new Rect(x, y, w, h));
            }
            else
            {
                for (int i = Layers.Count - 1; i >= 0; i--)
                {
                    if (!Layers[i].Visible)
                    {
                        continue;
                    }
                    var bmp = Layers[i].Bitmap.Clone();
                    if (opacity != null)
                    {
                        bmp.BlitRender(Layers[i].Bitmap, false, (float)opacity);
                    }
                    else
                    {
                        bmp.BlitRender(Layers[i].Bitmap, false, (float)Layers[i].Opacity);
                    }
                    dc.DrawImage(bmp, new Rect(x, y, w, h));
                }
            }
        }

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="c"></param>
        /// <param name="text"></param>
        /// <param name="dpi"></param>
        /// <param name="size"></param>
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

        /// <summary>
        /// Draws a X cross on the top of this frame element.
        /// </summary>
        /// <param name="dc"></param>
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

        /// <summary>
        /// Renders layers and other information such as <see cref="FrameName"/>.
        /// </summary>
        /// <param name="dc"></param>
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
                DrawLayers(dc);
            }

            var dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            DrawText(dc, 0, Height + 10, TextDrawColor, FrameName, dpi, 15);
            DrawText(dc, 0, Height + 25, TextDrawColor, Visible ? "Visible" : "Hidden", dpi, 15);

            if (_owner.ActiveFrameId == Id)
            {
                DrawBorder(dc, -5, -5, Width + 10, Height + 10);
            }
        }

        /// <summary>
        /// Renders layers and <see cref="FrameName"/> additionally onto the given <see cref="DrawingContext"/>.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="drawName">Indicates if <see cref="FrameName"/> should be drawn or not.</param>
        /// <param name="opacity">If provided, this opacity will be used for all drawn layers.</param>
        /// <param name="layerIndex">If provided, only the layer of this index will be drawn.</param>
        public void Render(DrawingContext dc, double w, double h, bool drawName = false, float? opacity = null, int layerIndex = -1)
        {
            if (!Visible)
            {
                return;
            }

            if (Layers != null && Layers.Count > 0)
            {
                DrawLayers(dc, opacity, layerIndex, baseWidth: w, baseHeight: h);
            }

            if (drawName)
            {
                DrawText(dc, 0, h + 10, TextDrawColor, FrameName, VisualTreeHelper.GetDpi(this).PixelsPerDip, 15);
            }
        }

        /// <summary>
        /// Creates a copy of this <see cref="DrawingFrame"/>.
        /// </summary>
        /// <param name="deep">
        /// Copy exact same name and <see cref="Guid"/> if set to true. Value of this parameter applies to the layers of this frame too.
        /// </param>
        /// <param name="sameName">Copy will have the same name.</param>
        /// <returns>Created copy.</returns>
        public DrawingFrame Clone(bool deep = false, bool sameName = false)
        {
            var layers = new List<DrawingLayer>();
            foreach (var l in Layers)
            {
                layers.Add(l.Clone(deep, true));
            }
            if (deep)
            {
                return new DrawingFrame(layers, FrameName, _owner, id: FrameGuid);
            }
            if (sameName)
            {
                return new DrawingFrame(layers, FrameName, _owner);
            }
            string newName;
            if (int.TryParse(FrameName, out int val))
            {
                newName = (++val).ToString();
            }
            else if (int.TryParse(FrameName.Split('_')[^1], out int val2))
            {
                newName = FrameName.Split('_')[0] + '_' + (++val2).ToString();
            }
            else
            {
                newName = FrameName + "_1";
            }
            return new DrawingFrame(layers, newName, _owner);
        }

        /// <summary>
        /// Gets bitmaps of each layer in <see cref="Layers"/>.
        /// </summary>
        /// <returns>Bitmaps.</returns>
        public IEnumerable<BitmapSource> GetLayersAsBitmapSources()
        {
            var bmps = new List<WriteableBitmap>();
            foreach (var layer in Layers)
            {
                bmps.Add(layer.GetWriteableBitmapWithAppliedOpacity());
            }
            return bmps;
        }

        /// <summary>
        /// Sets selected on left click, hides on right click, open <see cref="FrameName"/> settings on doubleclick.
        /// </summary>
        /// <param name="e"></param>
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
                    var strDoubleDialog = new StringInputDialog("Update Name Of The Frame", "Name");
                    if ((bool)strDoubleDialog.ShowDialog())
                    {
                        FrameName = strDoubleDialog.Answer;
                    }
                }
                else
                {
                    _owner.SetActiveFrame(this);
                }
            }
        }

        /// <summary>
        /// Restore frame state from memento.
        /// </summary>
        /// <param name="mem">Saved state.</param>
        public void Restore(FrameMemento mem)
        {
            _owner.HandleRestore(mem);
        }

        /// <summary>
        /// Generates a memento representing the current state of the frame.
        /// </summary>
        /// <param name="mTypeValue">Value of <see cref="IPixelEditorEventType"/>.</param>
        /// <returns>Resulting memento.</returns>
        public FrameMemento GetMemento(int mTypeValue)
        {
            return new FrameMemento(_owner, mTypeValue, _owner.ActiveFrameIndex, Clone(true));
        }

        /// <summary>
        /// Sets tiled mode for each layer in <see cref="Layers"/>.
        /// </summary>
        /// <param name="mode">Value indicating if tiled mode should be enabled on layers.</param>
        public void SetTiledMode(bool mode)
        {
            foreach (var l in Layers)
            {
                l.TiledModeEnabled = mode;
            }
        }

        /// <summary>
        /// Sets tile opacity for tiled mode for each layer in <see cref="Layers"/>.
        /// </summary>
        public void SetTiledModeOpacity(float opacity)
        {
            foreach (var l in Layers)
            {
                l.TiledModeOpacity = opacity;
            }
        }
    }
}