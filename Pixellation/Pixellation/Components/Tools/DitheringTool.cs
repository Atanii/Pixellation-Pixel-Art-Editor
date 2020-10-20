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

        private void Draw()
        {
            var p = Mouse.GetPosition(_layer);

            // odd X or even Y -> return
            if (((int)(p.X / _magnification) & 1) == 1 || ((int)(p.Y / _magnification) & 1) != 1)
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

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Draw();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Draw();
        }
    }
}