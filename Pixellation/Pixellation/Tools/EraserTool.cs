using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media;

namespace Pixellation.Tools
{
    public class EraserTool : BaseMultitonTool<EraserTool>
    {
        private new Color ToolColor = Color.FromArgb(0, 0, 0, 0);

        private EraserTool() : base()
        {
        }

        private void Draw(MouseEventArgs e)
        {
            SaveLayerMemento(true);

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            SetPixelWithThickness(_drawSurface, p.X, p.Y, ToolColor, Thickness);
            if (MirrorMode != MirrorModeStates.OFF)
            {
                p = Mirr(p);
                SetPixelWithThickness(_drawSurface, p.X, p.Y, ToolColor, Thickness);
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