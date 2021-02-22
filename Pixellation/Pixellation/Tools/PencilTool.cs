using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class PencilTool : BaseTool
    {
        private static PencilTool _instance;

        private PencilTool() : base()
        {
        }

        public static PencilTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PencilTool();
            }
            return _instance;
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