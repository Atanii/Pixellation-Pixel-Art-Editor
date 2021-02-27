using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class DitheringTool : BaseMultitonTool<DitheringTool>
    {
        private DitheringTool() : base()
        {
        }

        private void Draw(MouseEventArgs e)
        {
            SaveLayerMemento(true);

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            // odd X or even Y -> return
            if (e.LeftButton == MouseButtonState.Pressed && 
                (((p.X & 1) == 1 || (p.Y & 1) != 1) ^
                ((p.X & 1) != 1 || (p.Y & 1) == 1)) )
            {
                return;
            }
            // odd + odd ^ even + even
            if (e.LeftButton != MouseButtonState.Pressed &&
                (((p.X & 1) == 1 && (p.Y & 1) == 1) ^
                ((p.X & 1) != 1 && (p.Y & 1) != 1)) )
            {
                return;
            }

            if (OutOfBounds(p))
            {
                return;
            }

            _drawSurface.SetPixel(
                p.X,
                p.Y,
                ToolColor
            );
            if (MirrorMode != MirrorModeStates.OFF)
            {
                p = Mirr(p);
                _drawSurface.SetPixel(
                    p.X,
                    p.Y,
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
            {
                Draw(e);
            }
        }
    }
}