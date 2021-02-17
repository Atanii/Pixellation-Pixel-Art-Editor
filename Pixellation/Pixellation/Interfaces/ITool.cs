using Pixellation.Components.Editor;
using System.Drawing;
using System.Windows.Input;

namespace Pixellation.Components
{
    public interface ITool
    {
        public void SetDrawingCircumstances(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer);

        public void SetDrawColor(Color c);

        public Color GetDrawColor();

        public void OnMouseMove(MouseEventArgs e);

        public void OnMouseDown(MouseButtonEventArgs e);

        public void OnMouseUp(MouseButtonEventArgs e);

        public void OnKeyDown(KeyEventArgs e);

        public void Reset();
    }
}