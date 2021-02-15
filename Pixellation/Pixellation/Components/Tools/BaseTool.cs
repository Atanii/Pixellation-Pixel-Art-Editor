using Pixellation.Components.Editor;
using Pixellation.Utils;
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

        protected BaseTool()
        {
            ToolColor = System.Windows.Media.Color.FromRgb(0, 0, 0);
        }

        protected BaseTool(Color c)
        {
            ToolColor = c.ToMediaColor();
        }

        protected BaseTool(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer)
        {
            ToolColor = System.Windows.Media.Color.FromRgb(0, 0, 0);
            SetDrawingCircumstances(magnification, pixelWidth, pixelHeight, ds, previewLayer);
        }

        protected BaseTool(Color c, int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer)
        {
            ToolColor = c.ToMediaColor();
            SetDrawingCircumstances(magnification, pixelWidth, pixelHeight, ds, previewLayer);
        }

        protected void OnRaiseToolEvent(object sender, ToolEventArgs e)
        {
            RaiseToolEvent?.Invoke(sender, e);
        }

        public void SetDrawingCircumstances(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer)
        {
            this._magnification = magnification;
            this._surfaceWidth = pixelWidth * magnification;
            this._surfaceHeight = pixelHeight * magnification;
            this._layer = ds;
            this._previewLayer = previewLayer;
        }

        public void SetActiveLayer(DrawingLayer ds)
        {
            this._layer = ds;
        }

        public virtual Color GetDrawColor()
        {
            return ExtensionMethods.ToDrawingColor(ToolColor);
        }

        public virtual void SetDrawColor(Color c)
        {
            ToolColor = c.ToMediaColor();
        }

        public virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            return;
        }

        public virtual void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            return;
        }

        public virtual void OnMouseMove(MouseEventArgs e)
        {
            return;
        }

        public virtual void OnMouseRightButtonUp(MouseButtonEventArgs e)
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
    }
}