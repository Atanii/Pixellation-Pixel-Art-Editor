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

        private void Draw()
        {
            SaveLayerMemento(true);

            var p = Mouse.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            if (p.X < 0 || p.X >= (_surfaceWidth / _magnification) || p.Y < 0 || p.Y >= (_surfaceHeight / _magnification))
                return;

            _drawSurface.SetPixel(
                p.X, p.Y,
                ToolColor
            );
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            UnlockMemento();
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            Draw();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseDown(e))
                Draw();
        }
    }
}