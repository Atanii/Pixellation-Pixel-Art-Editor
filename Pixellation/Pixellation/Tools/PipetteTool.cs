using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class PipetteTool : BaseTool
    {
        private static PipetteTool _instance;

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
            var p = Mouse.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            if (p.X < 0 || p.X >= (_surfaceWidth / _magnification) || p.Y < 0 || p.Y >= (_surfaceHeight / _magnification))
                return;

            OnRaiseToolEvent(this, new ToolEventArgs { Type = e, Value = _drawSurface.GetPixel(p.X, p.Y).ToDrawingColor() });
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TakeColor(ToolEventType.PRIMARYCOLOR);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                TakeColor(ToolEventType.SECONDARY);
            }
        }
    }
}