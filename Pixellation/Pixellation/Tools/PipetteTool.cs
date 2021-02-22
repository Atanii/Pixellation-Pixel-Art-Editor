using Pixellation.Models;
using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class PipetteTool : BaseTool
    {
        private static PipetteTool _instance;

        private IntPoint p;

        private PipetteTool() : base()
        {
        }

        public static PipetteTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PipetteTool();
            }
            return _instance;
        }

        private void TakeColor(ToolEventType e)
        {
            OnRaiseToolEvent(this, new ToolEventArgs { Type = e, Value = _drawSurface.GetPixel(p.X, p.Y).ToDrawingColor() });
        }

        public override void OnMouseDown(ToolMouseEventArgs e)
        {
            p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
            if (!OutOfBounds(p) && e.LeftButton == MouseButtonState.Pressed)
            {
                TakeColor(ToolEventType.PRIMARYCOLOR);
            }
            if (!OutOfBounds(p) && e.RightButton == MouseButtonState.Pressed)
            {
                TakeColor(ToolEventType.SECONDARY);
            }
        }
    }
}