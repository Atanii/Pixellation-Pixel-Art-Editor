using Pixellation.Models;
using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class PipetteTool : BaseMultitonTool<PipetteTool>
    {
        private IntPoint p;

        private PipetteTool() : base()
        {
        }

        private void TakeColor(ToolEventType e)
        {
            OnRaiseToolEvent(this, new ToolEventArgs { Type = e, Value = _drawSurface.GetPixel(p.X, p.Y).ToDrawingColor() });
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
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