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
    public class DrawingFrame : FrameworkElement, IOriginator<FrameMemento, IPixelEditorEventType>
    {
        public Guid FrameGuid { get; private set; } = Guid.NewGuid();
        public string Id => FrameGuid.ToString();

        public static readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Color.FromArgb(25, 50, 50, 50));
        public static readonly Pen BackgroundPen = new Pen(BackgroundBrush, 1d);

        public static readonly SolidColorBrush BorderBrush = new SolidColorBrush(Color.FromArgb(255, 10, 10, 10));
        public static readonly Pen BorderPen = new Pen(BorderBrush, 1d);

        public static readonly Color TextDrawColor = Color.FromArgb(255, 0, 0, 0);

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
            get => MergeUtils.MergeAll(Layers);
        }

        #region Events

        public static event FrameEventHandler OnUpdated;

        #endregion Events

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
            PixelEditor.RaiseImageUpdatedEvent += (s, a) =>
            {
                InvalidateVisual();
            };
            Visible = visible;
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
                foreach (var l in Layers)
                {
                    if (!l.Visible)
                    {
                        continue;
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
        ///
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="drawName"></param>
        /// <param name="opacity"></param>
        /// <param name="layerIndex"></param>
        public void Render(DrawingContext dc, double x, double y, double w, double h, bool drawName = false, float? opacity = null, int layerIndex = -1)
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
        /// <returns>Created copy.</returns>
        public DrawingFrame Clone(bool deep = false)
        {
            var layers = new List<DrawingLayer>();
            foreach (var l in Layers)
            {
                layers.Add(l.Clone(deep));
            }
            if (deep)
            {
                return new DrawingFrame(layers, FrameName, _owner, id: FrameGuid);
            }
            return new DrawingFrame(layers, FrameName + "_copy", _owner);
        }

        public IEnumerable<BitmapSource> GetLayersAsBitmapSources()
        {
            var bmps = new List<WriteableBitmap>();
            foreach (var layer in Layers)
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

        public void Restore(FrameMemento mem)
        {
            _owner.HandleRestore(mem);
        }

        public FrameMemento GetMemento(int mTypeValue)
        {
            return new FrameMemento(_owner, mTypeValue, _owner.ActiveFrameIndex, Clone(true));
        }

        public void SetTiledMode(bool mode)
        {
            foreach (var l in Layers)
            {
                l.TiledModeOn = mode;
            }
        }

        public void SetTiledModeOpacity(float opacity)
        {
            foreach (var l in Layers)
            {
                l.TiledModeOpacity = opacity;
            }
        }
    }
}