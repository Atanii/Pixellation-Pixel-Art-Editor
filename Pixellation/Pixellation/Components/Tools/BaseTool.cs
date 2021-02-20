using Pixellation.Components.Editor;
using Pixellation.Utils;
using Pixellation.Utils.UndoRedo;
using System.Drawing;
using System.Windows.Input;

namespace Pixellation.Components.Tools
{
    public abstract class BaseTool : ITool
    {
        protected System.Windows.Media.Color ToolColor;

        protected int _magnification;
        protected int _surfaceWidth;
        protected int _surfaceHeight;
        protected DrawingLayer _layer;
        protected DrawingLayer _previewLayer;

        public delegate void ToolEventHandler(object sender, ToolEventArgs args);

        public static event ToolEventHandler RaiseToolEvent;

        private static Caretaker _mementoCaretaker;
        private bool _isMementoLocked = false;

        protected BaseTool()
        {
            _mementoCaretaker = Caretaker.GetInstance();
            ToolColor = System.Windows.Media.Color.FromRgb(0, 0, 0);
        }

        protected BaseTool(Color c)
        {
            _mementoCaretaker = Caretaker.GetInstance();
            ToolColor = c.ToMediaColor();
        }

        protected BaseTool(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer)
        {
            _mementoCaretaker = Caretaker.GetInstance();
            ToolColor = System.Windows.Media.Color.FromRgb(0, 0, 0);
            SetDrawingCircumstances(magnification, pixelWidth, pixelHeight, ds, previewLayer);
        }

        protected BaseTool(Color c, int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer)
        {
            _mementoCaretaker = Caretaker.GetInstance();
            ToolColor = c.ToMediaColor();
            SetDrawingCircumstances(magnification, pixelWidth, pixelHeight, ds, previewLayer);
        }

        protected void OnRaiseToolEvent(object sender, ToolEventArgs e)
        {
            RaiseToolEvent?.Invoke(sender, e);
        }

        public void SetDrawingCircumstances(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer)
        {
            _magnification = magnification;
            _surfaceWidth = pixelWidth * magnification;
            _surfaceHeight = pixelHeight * magnification;
            _layer = ds;
            _previewLayer = previewLayer;
        }

        public void SetActiveLayer(DrawingLayer ds)
        {
            _layer = ds;
        }

        public virtual Color GetDrawColor()
        {
            return ExtensionMethods.ToDrawingColor(ToolColor);
        }

        public virtual void SetDrawColor(Color c)
        {
            ToolColor = c.ToMediaColor();
        }

        protected bool IsMouseDown(MouseButtonEventArgs e) => e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed;
        protected bool IsMouseDown(MouseEventArgs e) => e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed;

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

        protected void SaveLayerMemento(bool lockMemento = false)
        {
            if (!_isMementoLocked)
            {
                _mementoCaretaker.Save(_layer.GetMemento(MementoType.DRAWONLAYER));
            }
            if (lockMemento)
            {
                _isMementoLocked = true;
            }
        }

        protected void UnlockMemento()
        {
            _isMementoLocked = false;
        }
    }
}