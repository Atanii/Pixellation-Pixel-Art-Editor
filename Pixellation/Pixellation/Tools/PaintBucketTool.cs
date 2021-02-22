using Pixellation.Models;
using Pixellation.Utils;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class PaintBucketTool : BaseMultitonTool<PaintBucketTool>
    {
        private PaintBucketTool() : base()
        {
        }

        private void Draw(ToolMouseEventArgs e)
        {
            SaveLayerMemento();

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            var targetColor = _layer.GetPixel(p.X, p.Y);
            var replacementColor = ToolColor;

            FloodFill(p, targetColor, replacementColor);
        }

        private void FloodFill(IntPoint point, System.Windows.Media.Color targetColor, System.Windows.Media.Color replacementColor)
        {
            if (targetColor == replacementColor)
            {
                return;
            }

            Stack<IntPoint> nodes = new Stack<IntPoint>();
            nodes.Push(point);

            IntPoint current;

            while (nodes.Count > 0)
            {
                current = nodes.Pop();

                if (_drawSurface.GetPixel(current.X, current.Y) != targetColor)
                {
                    continue;
                }
                else
                {
                    _drawSurface.SetPixel(
                        current.X, current.Y,
                        replacementColor
                    );

                    point = new IntPoint { X = current.X + 1, Y = current.Y };
                    if (point.X >= 0 && point.X < _surfaceWidth && point.Y >= 0 && point.Y < _surfaceHeight)
                    {
                        nodes.Push(point);
                    }

                    point = new IntPoint { X = current.X - 1, Y = current.Y };
                    if (point.X >= 0 && point.X < _surfaceWidth && point.Y >= 0 && point.Y < _surfaceHeight)
                    {
                        nodes.Push(point);
                    }

                    point = new IntPoint { X = current.X, Y = current.Y + 1 };
                    if (point.X >= 0 && point.X < _surfaceWidth && point.Y >= 0 && point.Y < _surfaceHeight)
                    {
                        nodes.Push(point);
                    }

                    point = new IntPoint { X = current.X, Y = current.Y - 1 };
                    if (point.X >= 0 && point.X < _surfaceWidth && point.Y >= 0 && point.Y < _surfaceHeight)
                    {
                        nodes.Push(point);
                    }
                }
            }
        }

        public override void OnMouseUp(ToolMouseEventArgs e)
        {
            Draw(e);
        }
    }
}