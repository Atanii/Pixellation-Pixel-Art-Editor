using Pixellation.Models;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Draws a rectangle.
    /// </summary>
    public class RectangleTool : BaseFourPointGeometryTool<RectangleTool>
    {
        private readonly Cursor _cursor = GetCursorFromResource("cursor-rectangle.cur");
        public override Cursor ToolCursor { get => _cursor; }

        private RectangleTool() : base()
        {
        }

        protected override void DrawGeometry(IntPoint p0, IntPoint p1, Color c, WriteableBitmap surface)
        {
            DrawRectangleWithThickness(p0.X, p0.Y, p1.X, p1.Y, surface, c, Thickness);

            if (MirrorMode != MirrorModeStates.OFF)
            {
                var p2 = Mirr(p0);
                var p3 = Mirr(p1);

                int x1 = Min(p2.X, p3.X);
                int y1 = Min(p2.Y, p3.Y);

                int x2 = Max(p2.X, p3.X);
                int y2 = Max(p2.Y, p3.Y);

                DrawRectangleWithThickness(x1, y1, x2, y2, surface, c, Thickness);
            }

            OnRaiseToolEvent(new ToolEventArgs());
        }

        private void DrawRectangleWithThickness(int x0, int y0, int x1, int y1, WriteableBitmap surface, Color c, ToolThickness thickness)
        {
            // Top
            for (int x = x0; x <= x1; x++)
            {
                SetPixelWithThickness(surface, x, y0, c, thickness);
            }
            // Bottom
            for (int x = x0; x <= x1; x++)
            {
                SetPixelWithThickness(surface, x, y1, c, thickness);
            }
            // Left
            for (int y = y0; y <= y1; y++)
            {
                SetPixelWithThickness(surface, x0, y, c, thickness);
            }
            // Right
            for (int y = y0; y <= y1; y++)
            {
                SetPixelWithThickness(surface, x1, y, c, thickness);
            }
        }
    }
}