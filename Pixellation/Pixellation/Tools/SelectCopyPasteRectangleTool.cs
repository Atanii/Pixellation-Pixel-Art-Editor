using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Copy-paste-cut tool using a rectangle as area.
    /// </summary>
    internal class SelectCopyPasteRectangleTool : BaseSelectionTool<SelectCopyPasteRectangleTool>
    {
        private readonly Cursor _cursor = GetCursorFromResource("cursor-selection-rectangle.cur");
        public override Cursor ToolCursor { get => _cursor; }

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