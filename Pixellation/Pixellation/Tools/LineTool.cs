using Pixellation.Models;
using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class LineTool : BaseMultitonTool<LineTool>
    {
        private IntPoint p0;
        private IntPoint p1;

        private bool _creating = false;

        private LineTool() : base()
        {
        }

        private void Draw()
        {
            SaveLayerMemento();
            _drawSurface.DrawLine(p0.X, p0.Y, p1.X, p1.Y, ToolColor);
            if (_mirrorModeState != MirrorModeStates.OFF)
            {
                var p2 = Mirr(p0);
                var p3 = Mirr(p1);
                _drawSurface.DrawLine(
                    p2.X, p2.Y, p3.X, p3.Y, ToolColor
                );
            }
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            p0 = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
            _creating = true;
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            Draw();
            _creating = false;
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseDown(e) && _creating)
            {
                p1 = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
                _previewDrawSurface.Clear();
                _previewDrawSurface.DrawLine(p0.X, p0.Y, p1.X, p1.Y, ToolColor);
                if (_mirrorModeState != MirrorModeStates.OFF)
                {
                    var p2 = Mirr(p0);
                    var p3 = Mirr(p1);
                    _previewDrawSurface.DrawLine(
                        p2.X, p2.Y, p3.X, p3.Y, ToolColor
                    );
                }
            }
        }
    }
}