using Pixellation.Models;
using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Picks the color of the clicked pixel.
    /// </summary>
    public class PipetteTool : BaseMultitonTool<PipetteTool>
    {
        private readonly Cursor _cursor = GetCursorFromResource("cursor-pipette.cur");
        public override Cursor ToolCursor { get => _cursor; }

        private IntPoint p;

        public override bool ThicknessCompatible => false;

        public override bool MirrorModeCompatible => false;

        private PipetteTool() : base()
        {
        }

        /// <summary>
        /// Picking color.
        /// </summary>
        /// <param name="e"></param>
        private void TakeColor(ToolEventType e)
        {
            OnRaiseToolEvent(new ToolEventArgs { Type = e, Value = _drawSurface.GetPixel(p.X, p.Y).ToDrawingColor() });
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
            if (!OutOfBounds(p) && e.LeftButton == MouseButtonState.Pressed)
            {
                TakeColor(ToolEventType.PRIMARYCOLOR);
                if (MirrorMode != MirrorModeStates.OFF)
                {
                    p = Mirr(p);
                    TakeColor(ToolEventType.SECONDARY);
                }
            }
            else if (!OutOfBounds(p) && e.RightButton == MouseButtonState.Pressed)
            {
                TakeColor(ToolEventType.SECONDARY);
                if (MirrorMode != MirrorModeStates.OFF)
                {
                    p = Mirr(p);
                    TakeColor(ToolEventType.PRIMARYCOLOR);
                }
            }
        }
    }
}