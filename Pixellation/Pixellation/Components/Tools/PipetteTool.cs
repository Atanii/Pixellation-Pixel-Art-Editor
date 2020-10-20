using Pixellation.Utils;
using System.Drawing;
using System.Windows.Input;

namespace Pixellation.Components.Tools
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

        private void TakeColor()
        {
            var p = Mouse.GetPosition(_layer);

            if (p.X < 0 || p.X >= _surfaceWidth || p.Y < 0 || p.Y >= _surfaceHeight)
                return;

            ToolColor = _layer.GetColor(
                (int)(p.X / _magnification),
                (int)(p.Y / _magnification));

            OnRaiseToolEvent(this, new ToolEventArgs { Type = ToolEventType.COLOR, Value = ToolColor.ToDrawingColor() });
        }

        public override void SetDrawColor(Color c)
        {
            return;
        }

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            TakeColor();
        }
    }
}