using System.Windows;
using System.Windows.Input;

namespace Pixellation.Tools
{
    public class ToolMouseEventArgs
    {
        public MouseEventArgs NestedMouseEventArgs { get; private set; }

        public MouseButtonState LeftButton => NestedMouseEventArgs.LeftButton;
        public MouseButtonState RightButton => NestedMouseEventArgs.RightButton;

        public MirrorModeStates MirrorModeState { get; set; }

        public double SurfaceWidth { get; set; }
        public double SurfaceHeight { get; set; }

        public ToolMouseEventArgs(MouseEventArgs nested)
        {
            NestedMouseEventArgs = nested;
        }

        public ToolMouseEventArgs()
        {
        }

        public void Set(MouseEventArgs e, double w, double h, MirrorModeStates mirrorModeState = MirrorModeStates.OFF)
        {
            NestedMouseEventArgs = e;
            SurfaceWidth = w;
            SurfaceHeight = h;
            MirrorModeState = mirrorModeState;
        }

        public Point GetPosition(IInputElement relativeTo)
        {
            var tmp = NestedMouseEventArgs.GetPosition(relativeTo);
            switch (MirrorModeState)
            {
                case MirrorModeStates.OFF:
                    return tmp;

                case MirrorModeStates.VERTICAL:
                    tmp.X = tmp.X >= (SurfaceWidth / 2) ?
                        SurfaceWidth - tmp.X : SurfaceWidth - tmp.X;
                    return tmp;

                case MirrorModeStates.HORIZONTAL:
                    tmp.Y = tmp.Y >= (SurfaceHeight / 2) ?
                        SurfaceHeight - tmp.Y : SurfaceHeight - tmp.Y;
                    return tmp;

                default:
                    return tmp;
            }
        }
    }
}