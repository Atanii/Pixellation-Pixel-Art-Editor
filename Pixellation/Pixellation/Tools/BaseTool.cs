using Pixellation.Components.Editor;
using Pixellation.Models;
using Pixellation.Utils;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    using MColor = System.Windows.Media.Color;

    public abstract class BaseTool
    {
        protected static int _magnification;

        protected static int _surfaceWidth;
        protected static int _surfaceHeight;

        protected static DrawingLayer _layer;
        protected static WriteableBitmap _drawSurface;

        protected static DrawingLayer _previewLayer;
        protected static WriteableBitmap _previewDrawSurface;

        public delegate void ToolEventHandler(object sender, ToolEventArgs args);

        public static event ToolEventHandler RaiseToolEvent;

        private static bool _isMementoLocked = false;

        private MColor _toolColor;

        public MColor ToolColor
        {
            get => EraserModeOn ? MColor.FromArgb(0, 0, 0, 0) : _toolColor;
            set => _toolColor = value;
        }

        public bool EraserModeOn { get; set; } = false;

        public MirrorModeStates MirrorMode { get; set; }

        public ToolThickness Thickness { get; set; }

        protected BaseTool()
        {
            ToolColor = MColor.FromRgb(0, 0, 0);
        }

        protected void OnRaiseToolEvent(object sender, ToolEventArgs e)
        {
            RaiseToolEvent?.Invoke(sender, e);
        }

        public void SetAllDrawingCircumstances(
            int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer, MirrorModeStates mirrorModeState = MirrorModeStates.OFF, ToolThickness thickness = ToolThickness.NORMAL)
        {
            _magnification = magnification;
            _surfaceWidth = pixelWidth;
            _surfaceHeight = pixelHeight;
            _layer = ds;
            _drawSurface = ds.Bitmap;
            _previewLayer = previewLayer;
            _previewDrawSurface = _previewLayer.Bitmap;
            MirrorMode = mirrorModeState;
            Thickness = thickness;
        }

        public void SetPixelSize(int pixelWidth, int pixelHeight)
        {
            _surfaceWidth = pixelWidth;
            _surfaceHeight = pixelHeight;
        }

        public void SetMagnification(int magnification)
        {
            _magnification = magnification;
        }

        public void SetActiveLayer(DrawingLayer layer)
        {
            _layer = layer;
            _drawSurface = _layer.Bitmap;
        }

        public void SetPreviewLayer(DrawingLayer layer)
        {
            _previewLayer = layer;
            _previewDrawSurface = _previewLayer.Bitmap;
        }

        public virtual Color GetDrawColor()
        {
            return ToolColor.ToDrawingColor();
        }

        public void SetDrawColor(Color c)
        {
            ToolColor = c.ToMediaColor();
        }

        public void SetMirrorMode(MirrorModeStates mirrorMode)
        {
            MirrorMode = mirrorMode;
        }

        protected static bool IsMouseDown(MouseButtonEventArgs e) => e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed;

        protected static bool IsMouseDown(MouseEventArgs e) => e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed;

        public virtual void OnMouseDown(MouseButtonEventArgs e)
        {
            return;
        }

        public virtual void OnMouseUp(MouseButtonEventArgs e)
        {
            return;
        }

        public virtual void OnMouseMove(MouseEventArgs e)
        {
            return;
        }

        public virtual void OnKeyDown(KeyEventArgs e)
        {
            return;
        }

        public virtual void Reset()
        {
            return;
        }

        protected static void SaveLayerMemento(bool lockMemento = false)
        {
            if (!_isMementoLocked)
            {
                _layer.SaveState(IPixelEditorEventType.LAYER_PIXELS_CHANGED);
            }
            if (lockMemento)
            {
                _isMementoLocked = true;
            }
        }

        protected static void UnlockMemento()
        {
            _isMementoLocked = false;
        }

        public static bool OutOfBounds(IntPoint p) => p.X < 0 || p.X >= _surfaceWidth || p.Y < 0 || p.Y >= _surfaceHeight;

        public static int Min(int a, int b) => a <= b ? a : b;

        public static int Max(int a, int b) => a >= b ? a : b;

        protected IntPoint MirrH(IntPoint p)
        {
            p.Y = _surfaceHeight - p.Y;
            return p;
        }

        protected IntPoint MirrV(IntPoint p)
        {
            p.X = _surfaceWidth - p.X;
            return p;
        }

        /// <summary>
        /// Mirrors the given point according to the current <see cref="MirrorModeStates"/>.
        /// In case MirrorMode is not turned on, it returns the point without any change.
        /// </summary>
        /// <param name="p">Point to mirror.</param>
        /// <returns>Mirrored point.</returns>
        protected IntPoint Mirr(IntPoint p)
        {
            switch (MirrorMode)
            {
                case MirrorModeStates.OFF:
                    break;

                case MirrorModeStates.VERTICAL:
                    p = MirrV(p);
                    break;

                case MirrorModeStates.HORIZONTAL:
                    p = MirrH(p);
                    break;

                default:
                    break;
            }
            return p;
        }

        public static void SetPixelWithThickness(WriteableBitmap bmp, int x0, int y0, MColor c, ToolThickness thickness)
        {
            if (x0 >= 0 && y0 >= 0 && x0 < bmp.PixelWidth && y0 < bmp.PixelHeight)
                bmp.SetPixel(x0, y0, c);

            if (thickness > ToolThickness.NORMAL)
            {
                if ((x0 - 1) >= 0 && y0 >= 0 && (x0 - 1) < bmp.PixelWidth && y0 < bmp.PixelHeight)
                    bmp.SetPixel(x0 - 1, y0, c);
                if ((x0 - 1) >= 0 && (y0 - 1) >= 0 && (x0 - 1) < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                    bmp.SetPixel(x0 - 1, y0 - 1, c);
                if (x0 >= 0 && (y0 - 1) >= 0 && x0 < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                    bmp.SetPixel(x0, y0 - 1, c);

                if (thickness > ToolThickness.MEDIUM)
                {
                    if ((x0 + 1) >= 0 && (y0 + 1) >= 0 && (x0 + 1) < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                        bmp.SetPixel(x0 + 1, y0 + 1, c);
                    if ((x0 + 1) >= 0 && (y0 - 1) >= 0 && (x0 + 1) < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                        bmp.SetPixel(x0 + 1, y0 - 1, c);
                    if ((x0 - 1) >= 0 && (y0 + 1) >= 0 && (x0 - 1) < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                        bmp.SetPixel(x0 - 1, y0 + 1, c);
                    if ((x0 + 1) >= 0 && y0 >= 0 && (x0 + 1) < bmp.PixelWidth && y0 < bmp.PixelHeight)
                        bmp.SetPixel(x0 + 1, y0, c);
                    if (x0 >= 0 && (y0 + 1) >= 0 && x0 < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                        bmp.SetPixel(x0, y0 + 1, c);
                }
            }
        }
    }
}