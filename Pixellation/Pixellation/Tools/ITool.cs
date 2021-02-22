using Pixellation.Components.Editor;
using System.Drawing;
using System.Windows.Input;
using static Pixellation.Tools.BaseTool;

namespace Pixellation.Tools
{
    public interface ITool
    {
        public static event ToolEventHandler RaiseToolEvent;

        public void SetAllDrawingCircumstances(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer);

        public void SetPixelSize(int pixelWidth, int pixelHeight);

        public void SetMagnification(int magnification);

        public void SetActiveLayer(DrawingLayer layer);

        public void SetPreviewLayer(DrawingLayer layer);

        public void SetDrawColor(Color c);

        public Color GetDrawColor();

        public void OnMouseMove(ToolMouseEventArgs e);

        public void OnMouseDown(ToolMouseEventArgs e);

        public void OnMouseUp(ToolMouseEventArgs e);

        public void OnKeyDown(KeyEventArgs e);

        public void Reset();
    }
}