using Pixellation.Utils;

namespace Pixellation.Tools
{
    public class EraserTool : BaseMultitonTool<EraserTool>
    {
        private EraserTool() : base()
        {
        }

        private void Draw(ToolMouseEventArgs e)
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

            _layer.InvalidateVisual();
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