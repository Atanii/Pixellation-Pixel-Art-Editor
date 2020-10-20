using System.Windows.Input;

namespace Pixellation.Components.Tools
{
    public class PaintBucketTool : BaseTool
    {
        private static PaintBucketTool _instance;

        private PaintBucketTool() : base()
        {
        }

        public static PaintBucketTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PaintBucketTool();
            }
            return _instance;
        }

        private void Draw()
        {
            var p = Mouse.GetPosition(_layer);

            var targetColor = _layer.GetColor((int)(p.X / _magnification), (int)(p.Y / _magnification));
            var replacementColor = ToolColor;

            RecursiveFloodFill(p, targetColor, replacementColor);

            _layer.InvalidateVisual();
        }

        private void RecursiveFloodFill(System.Windows.Point p, System.Windows.Media.Color targetColor, System.Windows.Media.Color replacementColor)
        {
            if (targetColor == replacementColor)
            {
                return;
            }
            else if (_layer.GetColor((int)(p.X / _magnification), (int)(p.Y / _magnification)) != targetColor)
            {
                return;
            }
            else if (p.X < 0 || p.X >= _surfaceWidth || p.Y < 0 || p.Y >= _surfaceHeight)
                return;
            else
            {
                _layer.SetColor(
                    (int)(p.X / _magnification),
                    (int)(p.Y / _magnification),
                    ToolColor);

                RecursiveFloodFill(new System.Windows.Point { X = p.X + _magnification, Y = p.Y }, targetColor, replacementColor);
                RecursiveFloodFill(new System.Windows.Point { X = p.X - _magnification, Y = p.Y }, targetColor, replacementColor);
                RecursiveFloodFill(new System.Windows.Point { X = p.X, Y = p.Y + _magnification }, targetColor, replacementColor);
                RecursiveFloodFill(new System.Windows.Point { X = p.X, Y = p.Y - _magnification }, targetColor, replacementColor);
            }
        }

        public override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Draw();
        }
    }
}