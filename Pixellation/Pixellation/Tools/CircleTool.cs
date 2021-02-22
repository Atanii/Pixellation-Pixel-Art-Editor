using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class CircleTool : BaseFourCornerGeometryTool<CircleTool>
    {
        private CircleTool() : base()
        {
        }

        protected override void DrawGeometry(int x1, int y1, int x2, int y2, Color c, WriteableBitmap surface)
        {
            surface.DrawEllipse(
                x1, y1, x2, y2, c
            );
        }
    }
}