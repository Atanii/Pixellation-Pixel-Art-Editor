using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class PencilTool : BaseMultitonTool<PencilTool>
    {
        private PencilTool() : base()
        {
        }

        private void Draw(MouseEventArgs e)
        {
            SaveLayerMemento(true);

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            if (OutOfBounds(p))
                return;

            _drawSurface.SetPixel(
                p.X, p.Y,
                ToolColor
            );
            if (_mirrorModeState != MirrorModeStates.OFF)
            {
                p = Mirr(p);
                _drawSurface.SetPixel(
                    p.X, p.Y,
                    ToolColor
                );
            }
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            UnlockMemento();
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            Draw(e);
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseDown(e))
                Draw(e);
        }
    }
}