using Pixellation.Components.Editor;
using System.Drawing;
using System.Windows.Input;

namespace Pixellation.Components
{
    public interface ITool
    {
        public void SetDrawingCircumstances(int magnification, int pixelWidth, int pixelHeight, DrawingSurface ds);

        public void SetDrawColor(Color c);

        public Color GetDrawColor();

        public void OnMouseMove(MouseEventArgs e);

        public void OnMouseLeftButtonDown(MouseButtonEventArgs e);

        public void OnMouseLeftButtonUp(MouseButtonEventArgs e);

        public void OnMouseRightButtonUp(MouseButtonEventArgs e);
    }
}