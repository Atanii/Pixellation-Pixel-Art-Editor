using Pixellation.Utils;
using System.Windows.Input;

namespace Pixellation.Tools
{
    /// <summary>
    /// Simple pencil.
    /// </summary>
    public class PencilTool : BaseMultitonTool<PencilTool>
    {
        private readonly Cursor _cursor = GetCursorFromResource("cursor-pencil.cur");
        public override Cursor ToolCursor { get => _cursor; }

        private PencilTool() : base()
        {
        }

        /// <summary>
        /// Drawing the clicked pixels.
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

        public override void OnMouseUp(MouseEventArgs e)
        {
            UnlockMemento();
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            Draw(e);
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseDown(e))
                Draw(e);
        }
    }
}