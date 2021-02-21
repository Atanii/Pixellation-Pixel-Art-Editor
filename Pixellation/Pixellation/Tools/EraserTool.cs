using Pixellation.Utils;
using System.Drawing;
using System.Windows.Input;

namespace Pixellation.Tools
{
    public class EraserTool : BaseTool
    {
        private static EraserTool _instance;

        private EraserTool() : base()
        {
        }

        public static EraserTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new EraserTool();
            }
            return _instance;
        }

        private void Draw()
        {
            SaveLayerMemento(true);

            var p = Mouse.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            if (OutOfBounds(p))
            {
                return;
            }

            _layer.SetPixel(
                p.X, p.Y,
                System.Windows.Media.Color.FromArgb(0,0,0,0)
            );

            _layer.InvalidateVisual();
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
            {
                Draw();
            }
        }
    }
}