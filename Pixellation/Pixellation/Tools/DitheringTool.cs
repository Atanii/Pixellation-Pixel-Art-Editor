using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class DitheringTool : BaseTool
    {
        private static DitheringTool _instance;

        private DitheringTool() : base()
        {
        }

        public static DitheringTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DitheringTool();
            }
            return _instance;
        }

        private void Draw(bool leftButtonPressed = true)
        {
            SaveLayerMemento(true);

            var p = Mouse.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            // odd X or even Y -> return
            if ( leftButtonPressed && 
                (((p.X & 1) == 1 || (p.Y & 1) != 1) ^
                ((p.X & 1) != 1 || (p.Y & 1) == 1)) )
            {
                return;
            }
            // odd + odd ^ even + even
            if ( !leftButtonPressed &&
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

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            UnlockMemento();
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            Draw(e.LeftButton == MouseButtonState.Pressed);
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Draw(true);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Draw(false);
            }
        }
    }
}