using Pixellation.Utils;
using System.Windows.Input;

namespace Pixellation.Tools
{
    public class EraserTool : BaseMultitonTool<EraserTool>
    {
        private EraserTool() : base()
        {
        }

        private void Draw(MouseEventArgs e)
        {
            SaveLayerMemento(true);

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            if (OutOfBounds(p))
            {
                return;
            }

            _layer.SetPixel(
                p.X, p.Y,
                System.Windows.Media.Color.FromArgb(0, 0, 0, 0)
            );
            if (_mirrorModeState != MirrorModeStates.OFF)
            {
                p = Mirr(p);
                _layer.SetPixel(
                    p.X, p.Y,
                    System.Windows.Media.Color.FromArgb(0, 0, 0, 0)
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
            {
                Draw(e);
            }
        }
    }
}