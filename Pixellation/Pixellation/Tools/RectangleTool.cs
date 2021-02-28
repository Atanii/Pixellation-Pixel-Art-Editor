using Pixellation.Models;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Draws a rectangle.
    /// </summary>
    public class RectangleTool : BaseFourCornerGeometryTool<RectangleTool>
    {
        private RectangleTool() : base()
        {
        }

        protected override void DrawGeometry(IntPoint p0, IntPoint p1, Color c, WriteableBitmap surface)
        {
            surface.DrawRectangle(p0.X, p0.Y, p1.X, p1.Y, c);
            if (Thickness > ToolThickness.NORMAL)
            {
                surface.DrawRectangle(p0.X + 1, p0.Y + 1, p1.X - 1, p1.Y - 1, c);
                if (Thickness > ToolThickness.MEDIUM)
                    surface.DrawRectangle(p0.X + 2, p0.Y + 2, p1.X - 2, p1.Y - 2, c);
            }

            if (MirrorMode != MirrorModeStates.OFF)
            {
                var p2 = Mirr(p0);
                var p3 = Mirr(p1);

                int x1 = Min(p2.X, p3.X);
                int y1 = Min(p2.Y, p3.Y);

                int x2 = Max(p2.X, p3.X);
                int y2 = Max(p2.Y, p3.Y);

                surface.DrawRectangle(x1, y1, x2, y2, c);
                if (Thickness > ToolThickness.NORMAL)
                {
                    surface.DrawRectangle(x1 + 1, y1 + 1, x2 - 1, y2 - 1, c);
                    if (Thickness > ToolThickness.MEDIUM)
                        surface.DrawRectangle(x1 + 2, y1 + 2, x2 - 2, y2 - 2, c);
                }
            }
        }
    }
}