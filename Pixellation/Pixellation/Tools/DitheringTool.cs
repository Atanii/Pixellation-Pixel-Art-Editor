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

        private void Draw(ToolMouseEventArgs e)
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
        }

        public override void OnMouseUp(ToolMouseEventArgs e)
        {
            UnlockMemento();
        }

        public override void OnMouseDown(ToolMouseEventArgs e)
        {
            Draw(e);
        }

        public override void OnMouseMove(ToolMouseEventArgs e)
        {
            if (IsMouseDown(e))
            {
                Draw(e);
            }
        }
    }
}