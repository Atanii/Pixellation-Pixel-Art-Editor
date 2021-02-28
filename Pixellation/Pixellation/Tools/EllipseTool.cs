using Pixellation.Models;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Draws an ellipse.
    /// </summary>
    public class EllipseTool : BaseFourCornerGeometryTool<EllipseTool>
    {
        private EllipseTool() : base()
        {
        }

        protected override void DrawGeometry(IntPoint p0, IntPoint p1, Color c, WriteableBitmap surface)
        {
            DrawEllipseWithThickness(p0.X, p0.Y, p1.X, p1.Y, surface, c, Thickness);
            if (MirrorMode != MirrorModeStates.OFF)
            {
                var p2 = Mirr(p0);
                var p3 = Mirr(p1);
                DrawEllipseWithThickness(
                    Min(p2.X, p3.X), Min(p2.Y, p3.Y), Max(p3.X, p2.X), Max(p3.Y, p2.Y),
                    surface, c, Thickness
                );
            }
        }

        /// <summary>
        /// Midpoint algorithm for drawing the ellipse.
        /// </summary>
        /// <param name="x0">X component of first point.</param>
        /// <param name="y0">Y component of first point.</param>
        /// <param name="x1">X component of second point.</param>
        /// <param name="y1">Y component of second point.</param>
        /// <param name="bmp">Bitmap to draw on.</param>
        /// <param name="c">Colors to draw with.</param>
        /// <param name="thickness">Thickness to draw with.</param>
        public static void DrawEllipseWithThickness(int x0, int y0, int x1, int y1, WriteableBitmap bmp, Color c, ToolThickness thickness)
        {
            double rx = (x1 - x0) / 2d;
            double ry = (y1 - y0) / 2d;
            double xc = x0 + (x1 - x0) / 2d;
            double yc = y0 + (y1 - y0) / 2d;

            double dx, dy, d1, d2, x, y;

            x = 0;
            y = ry;

            // Region 1?
            d1 = (ry * ry) - (rx * rx * ry) + (0.25f * rx * rx);
            dx = 2 * ry * ry * x;
            dy = 2 * rx * rx * y;

            // Region 1
            while (dx < dy)
            {
                int _x = (int)x;
                int _xc = (int)xc;
                int _y = (int)y;
                int _yc = (int)yc;

                // 4-way symmetry for 4 quadrants
                SetPixelWithThickness(bmp, _x + _xc, _y + _yc, c, thickness);
                SetPixelWithThickness(bmp, -_x + _xc, _y + _yc, c, thickness);
                SetPixelWithThickness(bmp, _x + _xc, -_y + _yc, c, thickness);
                SetPixelWithThickness(bmp, -_x + _xc, -_y + _yc, c, thickness);

                // Checking and updating value of decision parameter based on algorithm
                if (d1 < 0)
                {
                    x++;
                    dx = dx + (2 * ry * ry);
                    d1 = d1 + dx + (ry * ry);
                }
                else
                {
                    x++;
                    y--;
                    dx = dx + (2 * ry * ry);
                    dy = dy - (2 * rx * rx);
                    d1 = d1 + dx - dy + (ry * ry);
                }
            }

            // Region 2?
            d2 = ((ry * ry) * ((x + 0.5f) * (x + 0.5f)))
                + ((rx * rx) * ((y - 1) * (y - 1)))
                - (rx * rx * ry * ry);

            // Region 2
            while (y >= 0)
            {
                int _x = (int)x;
                int _xc = (int)xc;
                int _y = (int)y;
                int _yc = (int)yc;

                // 4-way symmetry for 4 quadrants
                SetPixelWithThickness(bmp, _x + _xc, _y + _yc, c, thickness);
                SetPixelWithThickness(bmp, -_x + _xc, _y + _yc, c, thickness);
                SetPixelWithThickness(bmp, _x + _xc, -_y + _yc, c, thickness);
                SetPixelWithThickness(bmp, -_x + _xc, -_y + _yc, c, thickness);

                // Checking and updating parameter value based on algorithm
                if (d2 > 0)
                {
                    y--;
                    dy = dy - (2 * rx * rx);
                    d2 = d2 + (rx * rx) - dy;
                }
                else
                {
                    y--;
                    x++;
                    dx = dx + (2 * ry * ry);
                    dy = dy - (2 * rx * rx);
                    d2 = d2 + dx - dy + (rx * rx);
                }
            }
        }
    }
}