using Pixellation.Utils;
using System.Drawing.Drawing2D;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    internal class SelectCopyPasteEllipseTool : BaseSelectionTool<SelectCopyPasteEllipseTool>
    {
        private SelectCopyPasteEllipseTool() : base()
        {
        }

        protected override void CutClear(int x1, int y1, int x2, int y2, Color c, WriteableBitmap surface)
        {
            System.Drawing.Rectangle boundingBox = new System.Drawing.Rectangle(x1, y1, x2, y2);
            GraphicsPath ellipse = new GraphicsPath();
            ellipse.AddEllipse(boundingBox);
            _copySrc.ClearPixelsByGraphicsPath(ellipse, false);

            surface.FillEllipse(
                    x1, y1, x2, y2, Colors.Transparent
            );
        }

        protected override void DrawSelection(int x1, int y1, int x2, int y2, Color c, WriteableBitmap surface)
        {
            surface.FillEllipse(
                x1, y1, x2, y2, c
            );
        }
    }
}