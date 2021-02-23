using Pixellation.Components.Editor;
using Pixellation.Models;
using Pixellation.Utils;
using Pixellation.MementoPattern;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public abstract class BaseTool
    {
        protected static System.Windows.Media.Color ToolColor;

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

        protected MirrorModeStates _mirrorModeState;

        protected BaseTool()
        {
            ToolColor = System.Windows.Media.Color.FromRgb(0, 0, 0);
        }

        protected void OnRaiseToolEvent(object sender, ToolEventArgs e)
        {
            RaiseToolEvent?.Invoke(sender, e);
        }

        public void SetAllDrawingCircumstances(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer, MirrorModeStates mirrorModeState = MirrorModeStates.OFF)
        {
            _magnification = magnification;
            _surfaceWidth = pixelWidth;
            _surfaceHeight = pixelHeight;
            _layer = ds;
            _drawSurface = ds.Bitmap;
            _previewLayer = previewLayer;
            _previewDrawSurface = _previewLayer.Bitmap;
            _mirrorModeState = mirrorModeState;
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
            _mirrorModeState = mirrorMode;
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
            switch (_mirrorModeState)
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
    }
}