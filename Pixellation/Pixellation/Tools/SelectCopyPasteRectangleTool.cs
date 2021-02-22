using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    internal class SelectCopyPasteRectangleTool : BaseSelectionTool<SelectCopyPasteRectangleTool>
    {
        private SelectCopyPasteRectangleTool() : base()
        {
        }

        protected override void CutClear(int x1, int y1, int x2, int y2, Color c, WriteableBitmap surface)
        {
            surface.FillRectangle(
                x1, y1, x2, y2, c
            );
        }

        protected override void DrawSelection(int x1, int y1, int x2, int y2, Color c, WriteableBitmap surface)
        {
            surface.FillRectangle(
                x1, y1, x2, y2, c
            );
        }
    }
}