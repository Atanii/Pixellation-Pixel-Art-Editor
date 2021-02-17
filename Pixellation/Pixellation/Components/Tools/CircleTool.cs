using Pixellation.Utils;
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
            _layer.GetWriteableBitmap().DrawEllipse((int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y, ToolColor);
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            p0 = e.GetPosition(_layer).IntDivide(_magnification, true);
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            Draw();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseDown(e))
            {
                p1 = e.GetPosition(_layer).IntDivide(_magnification, true);
                _previewLayer.GetWriteableBitmap().Clear();
                _previewLayer.GetWriteableBitmap().DrawEllipse((int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y, ToolColor);
            }   
        }
    }
}