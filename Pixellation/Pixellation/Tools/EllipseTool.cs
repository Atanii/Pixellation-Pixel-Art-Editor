using Pixellation.Models;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class EllipseTool : BaseFourCornerGeometryTool<EllipseTool>
    {
        private EllipseTool() : base()
        {
        }

        protected override void DrawGeometry(IntPoint p0, IntPoint p1, Color c, WriteableBitmap surface)
        {
            surface.DrawEllipse(
                p0.X, p0.Y, p1.X, p1.Y, c
            );
            if (_mirrorModeState != MirrorModeStates.OFF)
            {
                var p2 = Mirr(p0);
                var p3 = Mirr(p1);
                surface.DrawEllipse(
                    Min(p2.X, p3.X), Min(p2.Y, p3.Y), Max(p3.X, p2.X), Max(p3.Y, p2.Y), c
                );
            }
        }
    }
}