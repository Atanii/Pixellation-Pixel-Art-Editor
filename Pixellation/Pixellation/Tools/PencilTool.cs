using Pixellation.Utils;
using System.Windows.Input;

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
                Draw(e);
        }
    }
}