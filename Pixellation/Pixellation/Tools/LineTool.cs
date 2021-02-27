using Pixellation.Models;
using Pixellation.Utils;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class LineTool : BaseMultitonTool<LineTool>
    {
        private IntPoint p0;
        private IntPoint p1;

        private bool _creating = false;

        private LineTool() : base()
        {
        }

        private void Draw()
        {
            SaveLayerMemento();

            var drawArea = _drawSurface.GetDrawArea();
            _drawSurface.Blit(
                drawArea,
                _previewDrawSurface,
                drawArea,
                WriteableBitmapExtensions.BlendMode.Alpha
            );
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            p0 = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);
            _creating = true;
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            Draw();
            _creating = false;
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseDown(e) && _creating)
            {
                p1 = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

                _previewDrawSurface.Clear();

                DrawLineWithThickness(_previewDrawSurface, p0, p1, Thickness, ToolColor);

                if (MirrorMode != MirrorModeStates.OFF)
                {
                    var p2 = Mirr(p0);
                    var p3 = Mirr(p1);

                    DrawLineWithThickness(_previewDrawSurface, p2, p3, Thickness, ToolColor);
                }
            }
        }

        public static void DrawLineWithThickness(WriteableBitmap bitmap, IntPoint p0, IntPoint p1, ToolThickness thickness, Color color)
        {
            int x0 = p0.X, x1 = p1.X, y0 = p0.Y, y1 = p1.Y;

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;

            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;

            int err = dx + dy;
            int e2 = 0;

            while (true)
            {
                SetPixelWithThickness(bitmap, x0, y0, color, thickness);

                if (x0 == x1 && y0 == y1)
                {
                    break;
                }

                e2 = 2 * err;

                if (e2 >= dy)
                {
                    err += dy;
                    x0 += sx;
                }
                if (e2 <= dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
    }
}