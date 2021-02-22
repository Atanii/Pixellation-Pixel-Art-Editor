using Pixellation.Utils;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class PencilTool : BaseMultitonTool<PencilTool>
    {
        private PencilTool() : base()
        {
        }

        private void Draw(ToolMouseEventArgs e)
        {
            SaveLayerMemento(true);

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            if (OutOfBounds(p))
                return;

            _drawSurface.SetPixel(
                p.X, p.Y,
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
                Draw(e);
        }
    }
}