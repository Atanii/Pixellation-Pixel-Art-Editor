using Pixellation.Models;
using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Base class for all four-point geometry drawing tools in Pixellation.
    /// </summary>
    /// <typeparam name="T">Type of inheriting class. Needed for multiton pattern.</typeparam>
    public abstract class BaseFourPointGeometryTool<T> : BaseMultitonTool<T> where T : class, ITool
    {
        private IntPoint p0;
        private IntPoint p1;

        private bool _creating = false;
        private bool _dragging = false;

        protected BaseFourPointGeometryTool() : base()
        {
        }

        private void Draw()
        {
            SaveLayerMemento();
            var _p0 = new IntPoint(Min(p0.X, p1.X), Min(p0.Y, p1.Y));
            var _p1 = new IntPoint(Max(p0.X, p1.X), Max(p0.Y, p1.Y));
            DrawGeometry(_p0, _p1, ToolColor, _drawSurface);
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            p0 = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
            _creating = true;
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            if (_creating && _dragging)
            {
                Draw();
            }
            _dragging = false;
            _creating = false;
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseDown(e))
            {
                _dragging = true;
                if (_creating && _dragging)
                {
                    p1 = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
                    _previewDrawSurface.Clear();
                    var _p0 = new IntPoint(Min(p0.X, p1.X), Min(p0.Y, p1.Y));
                    var _p1 = new IntPoint(Max(p0.X, p1.X), Max(p0.Y, p1.Y));
                    DrawGeometry(_p0, _p1, ToolColor, _previewDrawSurface);
                }
            }
        }

        /// <summary>
        /// Draws the four-point geometry.
        /// </summary>
        /// <param name="p0">First point.</param>
        /// <param name="p1">Second point.</param>
        /// <param name="c">Color to draw geometry with.</param>
        /// <param name="surface">Bitmap to draw geometry on.</param>
        protected abstract void DrawGeometry(IntPoint p0, IntPoint p1, Color c, WriteableBitmap surface);
    }
}
