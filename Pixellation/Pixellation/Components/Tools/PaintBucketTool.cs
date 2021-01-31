using System.Collections.Generic;
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

            FloodFill(p, targetColor, replacementColor);

            _layer.InvalidateVisual();
        }

        private void FloodFill(System.Windows.Point startingPoint, System.Windows.Media.Color targetColor, System.Windows.Media.Color replacementColor)
        {
            if (targetColor == replacementColor)
            {
                return;
            }

            Stack<System.Windows.Point> nodes = new Stack<System.Windows.Point>();
            nodes.Push(startingPoint);
            
            System.Windows.Point current;

            while (nodes.Count > 0)
            {
                current = nodes.Pop();

                if (_layer.GetColor((int)(current.X / _magnification), (int)(current.Y / _magnification)) != targetColor)
                {
                    continue;
                }
                else
                {
                    _layer.SetColor(
                        (int)(current.X / _magnification),
                        (int)(current.Y / _magnification),
                        replacementColor
                    );

                    var tmp = new System.Windows.Point { X = current.X + _magnification, Y = current.Y };
                    if (tmp.X >= 0 && tmp.X < _surfaceWidth && tmp.Y >= 0 && tmp.Y < _surfaceHeight)
                    {
                        nodes.Push(tmp);
                    }

                    tmp = new System.Windows.Point { X = current.X - _magnification, Y = current.Y };
                    if (tmp.X >= 0 && tmp.X < _surfaceWidth && tmp.Y >= 0 && tmp.Y < _surfaceHeight)
                    {
                        nodes.Push(tmp);
                    }

                    tmp = new System.Windows.Point { X = current.X, Y = current.Y + _magnification };
                    if (tmp.X >= 0 && tmp.X < _surfaceWidth && tmp.Y >= 0 && tmp.Y < _surfaceHeight)
                    {
                        nodes.Push(tmp);
                    }

                    tmp = new System.Windows.Point { X = current.X, Y = current.Y - _magnification };
                    if (tmp.X >= 0 && tmp.X < _surfaceWidth && tmp.Y >= 0 && tmp.Y < _surfaceHeight)
                    {
                        nodes.Push(tmp);
                    }
                }
            }
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