using Pixellation.Components.Editor;
using Pixellation.Models;
using Pixellation.Utils;
using Pixellation.Utils.MementoPattern;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public abstract class BaseTool : ITool
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

        private static readonly Caretaker<IPixelEditorEventType> _mementoCaretaker = Caretaker<IPixelEditorEventType>.GetInstance();
        private static bool _isMementoLocked = false;

        protected BaseTool()
        {
            ToolColor = System.Windows.Media.Color.FromRgb(0, 0, 0);
        }

        protected void OnRaiseToolEvent(object sender, ToolEventArgs e)
        {
            RaiseToolEvent?.Invoke(sender, e);
        }

        public void SetAllDrawingCircumstances(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer)
        {
            _magnification = magnification;
            _surfaceWidth = pixelWidth;
            _surfaceHeight = pixelHeight;
            _layer = ds;
            _drawSurface = ds.Bitmap;
            _previewLayer = previewLayer;
            _previewDrawSurface = _previewLayer.Bitmap;
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

        protected static bool IsMouseDown(ToolMouseEventArgs e) => e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed;

        public virtual void OnMouseDown(ToolMouseEventArgs e)
        {
            return;
        }

        public virtual void OnMouseUp(ToolMouseEventArgs e)
        {
            return;
        }

        public virtual void OnMouseMove(ToolMouseEventArgs e)
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
                _mementoCaretaker.Save(_layer.GetMemento(IPixelEditorEventType.LAYER_PIXELS_CHANGED));
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
    }
}