using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Tools
{
    public class RectangleTool : BaseTool
    {
        private static RectangleTool _instance;

        private System.Windows.Point p0;
        private System.Windows.Point p1;

        private RectangleTool() : base()
        {
        }

        public static RectangleTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new RectangleTool();
            }
            return _instance;
        }

        private void Draw()
        {
            _layer.GetWriteableBitmap().DrawRectangle((int)p0.X / _magnification, (int)p0.Y / _magnification, (int)p1.X / _magnification, (int)p1.Y / _magnification, ToolColor);
        }

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            p0 = Mouse.GetPosition(_layer);
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Draw();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                p1 = Mouse.GetPosition(_layer);
        }
    }
}