using System.Windows.Input;

namespace Pixellation.Components.Tools
{
    public class LineTool : BaseTool
    {
        private static LineTool _instance;

        private System.Windows.Point p0;
        private System.Windows.Point p1;

        private LineTool() : base()
        {
        }

        public static LineTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LineTool();
            }
            return _instance;
        }

        private void Draw()
        {
            // Equidistant points between two coordinates:
            // x = x1 + (x2 - x1) * t
            // y = y1 + (y2 - y1) * t

            for (int t = 0; t < 100; t++)
            {
                var _t = t / 100.0d;
                var x = p0.X + (p1.X - p0.X) * _t;
                var y = p0.Y + (p1.Y - p0.Y) * _t;

                if (x < 0 || x >= _surfaceWidth || y < 0 || y >= _surfaceHeight)
                    continue;

                _layer.SetColor(
                    (int)(x / _magnification),
                    (int)(y / _magnification),
                    ToolColor);
            }

            _layer.InvalidateVisual();
        }

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            p0 = Mouse.GetPosition(_layer);
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Draw();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                p1 = Mouse.GetPosition(_layer);
        }
    }
}