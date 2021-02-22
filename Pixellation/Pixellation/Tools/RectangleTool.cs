using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class RectangleTool : BaseFourCornerGeometryTool<RectangleTool>
    {
        private RectangleTool() : base()
        {
        }

        protected override void DrawGeometry(int x1, int y1, int x2, int y2, Color c, WriteableBitmap surface)
        {
            surface.DrawRectangle(
                x1, y1, x2, y2, c
            );
        }
    }
}