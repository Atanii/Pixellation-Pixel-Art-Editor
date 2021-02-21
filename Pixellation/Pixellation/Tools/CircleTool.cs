using Pixellation.Models;
using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class CircleTool : BaseTool
    {
        private static CircleTool _instance;

        private IntPoint p0;
        private IntPoint p1;

        private bool _creating = false;
        private bool _dragging = false;

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
            SaveLayerMemento();
            _drawSurface.DrawEllipse(
                Min(p0.X, p1.X),
                Min(p0.Y, p1.Y),
                Max(p0.X, p1.X),
                Max(p0.Y, p1.Y),
                ToolColor
            );
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            p0 = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
            _creating = true;
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (_creating && _dragging)
            {
                Draw();
            }
            _dragging = false;
            _creating = false;
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseDown(e))
            {
                _dragging = true;
                if (_creating && _dragging)
                {
                    p1 = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
                    _previewDrawSurface.Clear();
                    _previewDrawSurface.DrawEllipse(
                        Min(p0.X, p1.X),
                        Min(p0.Y, p1.Y),
                        Max(p0.X, p1.X),
                        Max(p0.Y, p1.Y),
                        ToolColor
                    );
                }
            }   
        }
    }
}