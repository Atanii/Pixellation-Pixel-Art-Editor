using Pixellation.Models;
using Pixellation.Utils;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Fills the clicked area consisting of the clicked color with the currently set color.
    /// </summary>
    public class PaintBucketTool : BaseMultitonTool<PaintBucketTool>
    {
        private readonly Cursor _cursor = GetCursorFromResource("cursor-paintbucket.cur");

        public override Cursor ToolCursor { get => _cursor; }

        public override bool MirrorModeCompatible => false;

        public override bool ThicknessCompatible => false;

        private PaintBucketTool() : base()
        {
        }

        /// <summary>
        /// Getting the cliked point of the area.
        /// </summary>
        /// <param name="e"></param>
        private void Draw(MouseEventArgs e)
        {
            SaveLayerMemento();

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            var targetColor = _drawSurface.GetPixel(p.X, p.Y);
            var replacementColor = ToolColor;

            FloodFill(p, targetColor, replacementColor);

            OnRaiseToolEvent(new ToolEventArgs());
        }

        /// <summary>
        /// Implementation of the FloodFill algorithm for coloring the area.
        /// </summary>
        /// <param name="point">Starting point of the area to color.</param>
        /// <param name="targetColor">Color to replace.</param>
        /// <param name="replacementColor">Color with replace.</param>
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

        public override void OnMouseUp(MouseEventArgs e)
        {
            Draw(e);
        }
    }
}