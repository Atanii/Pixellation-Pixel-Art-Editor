using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media;

namespace Pixellation.Tools
{
    /// <summary>
    /// Simple eraser.
    /// </summary>
    public class EraserTool : BaseMultitonTool<EraserTool>
    {
        private readonly Cursor _cursor = GetCursorFromResource("cursor-eraser.cur");
        public override Cursor ToolCursor { get => _cursor; }

        /// <summary>
        /// Used color is always transparent for <see cref="EraserTool"/>.
        /// </summary>
        private new Color ToolColor = Color.FromArgb(0, 0, 0, 0);

        private EraserTool() : base()
        {
        }

        /// <summary>
        /// Erase clicked pixels.
        /// </summary>
        /// <param name="e"></param>
        private void Draw(MouseEventArgs e)
        {
            SaveLayerMemento(true);

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            SetPixelWithThickness(_drawSurface, p.X, p.Y, ToolColor, Thickness);
            if (MirrorMode != MirrorModeStates.OFF)
            {
                p = Mirr(p);
                SetPixelWithThickness(_drawSurface, p.X, p.Y, ToolColor, Thickness);
            }
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            UnlockMemento();
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            Draw(e);
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseDown(e))
            {
                Draw(e);
            }
        }
    }
}