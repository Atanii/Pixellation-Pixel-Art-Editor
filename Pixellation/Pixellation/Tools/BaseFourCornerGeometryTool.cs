using Pixellation.Models;
using Pixellation.Utils;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public abstract class BaseFourCornerGeometryTool<T> : BaseMultitonTool<T> where T : class
    {
        private IntPoint p0;
        private IntPoint p1;

        private bool _creating = false;
        private bool _dragging = false;

        protected BaseFourCornerGeometryTool() : base()
        {
        }

        private void Draw()
        {
            SaveLayerMemento();
            DrawGeometry(Min(p0.X, p1.X), Min(p0.Y, p1.Y), Max(p0.X, p1.X), Max(p0.Y, p1.Y), ToolColor, _drawSurface);
        }

        public override void OnMouseDown(ToolMouseEventArgs e)
        {
            p0 = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
            _creating = true;
        }

        public override void OnMouseUp(ToolMouseEventArgs e)
        {
            if (_creating && _dragging)
            {
                Draw();
            }
            _dragging = false;
            _creating = false;
        }

        public override void OnMouseMove(ToolMouseEventArgs e)
        {
            if (IsMouseDown(e))
            {
                _dragging = true;
                if (_creating && _dragging)
                {
                    p1 = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
                    _previewDrawSurface.Clear();
                    DrawGeometry(Min(p0.X, p1.X), Min(p0.Y, p1.Y), Max(p0.X, p1.X), Max(p0.Y, p1.Y), ToolColor, _previewDrawSurface);
                }
            }
        }

        protected abstract void DrawGeometry(int x1, int y1, int x2, int y2, Color c, WriteableBitmap surface);
    }
}
