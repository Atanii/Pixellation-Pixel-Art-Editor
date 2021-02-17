using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Tools
{
    public class LineTool : BaseTool
    {
        private static LineTool _instance;

        private System.Windows.Point p0;
        private System.Windows.Point p1;

        private LineTool() : base()
        {
        }

        public static LineTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LineTool();
            }
            return _instance;
        }

        private void Draw()
        {
            _layer.GetWriteableBitmap().DrawLine((int)p0.X / _magnification, (int)p0.Y / _magnification, (int)p1.X / _magnification, (int)p1.Y / _magnification, ToolColor);
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            p0 = e.GetPosition(_layer);
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            Draw();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (p0 != null && IsMouseDown(e))
            {   
                p1 = e.GetPosition(_layer);
                _previewLayer.GetWriteableBitmap().Clear();
                _previewLayer.GetWriteableBitmap().DrawLine((int)p0.X / _magnification, (int)p0.Y / _magnification, (int)p1.X / _magnification, (int)p1.Y / _magnification, ToolColor);
            }
        }
    }
}