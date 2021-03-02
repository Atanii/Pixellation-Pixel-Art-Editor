using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Tool for balancing pixels. With left click it decrease the maximum from the RGB of the clicked pixel, on right click the opposite.
    /// </summary>
    public class BalancerTool : BaseMultitonTool<BalancerTool>
    {
        private readonly Cursor _cursor = GetCursorFromResource("cursor-balancer.cur");
        public override Cursor ToolCursor { get => _cursor; }

        public override bool EraserModeCompatible => false;

        private BalancerTool() : base()
        {
        }

        /// <summary>
        /// Lightening or darkening the clicked pixels.
        /// </summary>
        /// <param name="e"></param>
        private void Draw(MouseEventArgs e)
        {
            SaveLayerMemento(true);

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            byte modifier = 10;
            bool max = true;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                max = false;
            }

            BalancePixelWithThickness(_drawSurface, p.X, p.Y, Thickness, modifier, max);
            if (MirrorMode != MirrorModeStates.OFF)
            {
                p = Mirr(p);
                BalancePixelWithThickness(_drawSurface, p.X, p.Y, Thickness, modifier, max);
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
                Draw(e);
        }

        /// <summary>
        /// Balance one unit of draw area (default is 1px).
        /// Based on the given thickness it can affect the surrounding pixels making a bigger draw "unit".
        /// </summary>
        /// <param name="bmp">Bitmap to draw on.</param>
        /// <param name="x0">X component of coordinate.</param>
        /// <param name="y0">Y component of coordinate.</param>
        /// <param name="thickness">Thickness to use.</param>
        /// <param name="modifier">Value to balance with.</param>
        /// <param name="max">Add to max or subtract from min RGB.</param>
        public static void BalancePixelWithThickness(WriteableBitmap bmp, int x0, int y0, ToolThickness thickness, byte modifier, bool max)
        {
            if (x0 >= 0 && y0 >= 0 && x0 < bmp.PixelWidth && y0 < bmp.PixelHeight)
            {
                bmp.SetPixel(x0, y0, ColorUtils.BalanceColor(bmp.GetPixel(x0, y0), max, modifier));
            }

            if (thickness > ToolThickness.NORMAL)
            {
                if ((x0 - 1) >= 0 && y0 >= 0 && (x0 - 1) < bmp.PixelWidth && y0 < bmp.PixelHeight)
                {
                    bmp.SetPixel(x0 - 1, y0, ColorUtils.BalanceColor(bmp.GetPixel(x0 - 1, y0), max, modifier));
                }
                if ((x0 - 1) >= 0 && (y0 - 1) >= 0 && (x0 - 1) < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                {
                    bmp.SetPixel(x0 - 1, y0 - 1, ColorUtils.BalanceColor(bmp.GetPixel(x0 - 1, y0 - 1), max, modifier));
                }
                if (x0 >= 0 && (y0 - 1) >= 0 && x0 < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                {
                    bmp.SetPixel(x0, y0 - 1, ColorUtils.BalanceColor(bmp.GetPixel(x0, y0 - 1), max, modifier));
                }

                if (thickness > ToolThickness.MEDIUM)
                {
                    if ((x0 + 1) >= 0 && (y0 + 1) >= 0 && (x0 + 1) < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                    {
                        bmp.SetPixel(x0 + 1, y0 + 1, ColorUtils.BalanceColor(bmp.GetPixel(x0 + 1, y0 + 1), max, modifier));
                    }
                    if ((x0 + 1) >= 0 && (y0 - 1) >= 0 && (x0 + 1) < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                    {
                        bmp.SetPixel(x0 + 1, y0 - 1, ColorUtils.BalanceColor(bmp.GetPixel(x0 + 1, y0 - 1), max, modifier));
                    }
                    if ((x0 - 1) >= 0 && (y0 + 1) >= 0 && (x0 - 1) < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                    {
                        bmp.SetPixel(x0 - 1, y0 + 1, ColorUtils.BalanceColor(bmp.GetPixel(x0 - 1, y0 + 1), max, modifier));
                    }
                    if ((x0 + 1) >= 0 && y0 >= 0 && (x0 + 1) < bmp.PixelWidth && y0 < bmp.PixelHeight)
                    {
                        bmp.SetPixel(x0 + 1, y0, ColorUtils.BalanceColor(bmp.GetPixel(x0 + 1, y0), max, modifier));
                    }
                    if (x0 >= 0 && (y0 + 1) >= 0 && x0 < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                    {
                        bmp.SetPixel(x0, y0 + 1, ColorUtils.BalanceColor(bmp.GetPixel(x0, y0 + 1), max, modifier));
                    }
                }
            }
        }
    }
}
