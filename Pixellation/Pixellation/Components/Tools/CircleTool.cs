using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Tools
{
    public class CircleTool : BaseTool
    {
        private static CircleTool _instance;

        private System.Windows.Point p0;
        private System.Windows.Point p1;

        private CircleTool() : base()
        {
        }

        public static CircleTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CircleTool();
            }
            return _instance;
        }

        private void Draw()
        {
            _layer.GetWriteableBitmap().DrawEllipse((int)p0.X / _magnification, (int)p0.Y / _magnification, (int)p1.X / _magnification, (int)p1.Y / _magnification, ToolColor);
        }

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            p0 = e.GetPosition(_layer);
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Draw();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                p1 = e.GetPosition(_layer);
                _previewLayer.GetWriteableBitmap().Clear();
                _previewLayer.GetWriteableBitmap().DrawEllipse((int)p0.X / _magnification, (int)p0.Y / _magnification, (int)p1.X / _magnification, (int)p1.Y / _magnification, ToolColor);
            }   
        }
    }
}