using System.Windows.Input;

namespace Pixellation.Components.Tools
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

            var p = Mouse.GetPosition(_layer);

            // odd X or even Y -> return
            if ( leftButtonPressed && 
                ((((int)(p.X / _magnification) & 1) == 1 || ((int)(p.Y / _magnification) & 1) != 1) ^
                (((int)(p.X / _magnification) & 1) != 1 || ((int)(p.Y / _magnification) & 1) == 1))
            )
            {
                return;
            }
            // odd + odd ^ even + even
            if ( !leftButtonPressed &&
                ((((int)( p.X / _magnification) & 1) == 1 && ((int)( p.Y / _magnification) & 1) == 1) ^
                (((int)( p.X / _magnification) & 1) != 1 && ((int)( p.Y / _magnification) & 1) != 1))
            )
            {
                return;
            }

            if (p.X < 0 || p.X >= _surfaceWidth || p.Y < 0 || p.Y >= _surfaceHeight)
                return;

            _layer.SetColor(
                (int)(p.X / _magnification),
                (int)(p.Y / _magnification),
                ToolColor);

            _layer.InvalidateVisual();
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