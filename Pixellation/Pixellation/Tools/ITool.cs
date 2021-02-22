using Pixellation.Components.Editor;
using System.Drawing;
using System.Windows.Input;

namespace Pixellation.Tools
{
    public interface ITool
    {
        public void SetAllDrawingCircumstances(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer);

        public void SetDrawColor(Color c);

        public Color GetDrawColor();

        public void OnMouseMove(ToolMouseEventArgs e);

        public void OnMouseDown(ToolMouseEventArgs e);

        public void OnMouseUp(ToolMouseEventArgs e);

        public void OnKeyDown(KeyEventArgs e);

        public void Reset();
    }
}