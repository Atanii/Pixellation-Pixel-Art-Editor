using Pixellation.Models;
using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class RectangleTool : BaseTool
    {
        private static RectangleTool _instance;

        private IntPoint p0;
        private IntPoint p1;

        private bool _creating = false;
        private bool _dragging = false;

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
            SaveLayerMemento();
            _drawSurface.DrawRectangle(
                Min(p0.X, p1.X),
                Min(p0.Y, p1.Y),
                Max(p0.X, p1.X),
                Max(p0.Y, p1.Y),
                ToolColor
            );
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
                    _previewDrawSurface.DrawRectangle(
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