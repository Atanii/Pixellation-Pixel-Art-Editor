using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    using MColor = System.Windows.Media.Color;

    /// <summary>
    /// Delegate for test to decide if checkboard pattern part should be drawn or not to the given coordinate.
    /// </summary>
    /// <param name="x">X component of coordinate.</param>
    /// <param name="y">Y component of coordinate.</param>
    /// <returns>True if drawable, false otherwise.</returns>
    public delegate bool DitheringTest(int x, int y);

    /// <summary>
    /// Tool for "chess-table" pattern drawing.
    /// </summary>
    public class DitheringTool : BaseMultitonTool<DitheringTool>
    {
        private readonly Cursor _cursor = GetCursorFromResource("cursor-dithering.cur");

        public override Cursor ToolCursor { get => _cursor; }

        private DitheringTool() : base()
        {
        }

        /// <summary>
        /// Test for left click dithering.
        /// </summary>
        /// <param name="x">X component of coordinate.</param>
        /// <param name="y">Y component of coordinate.</param>
        /// <returns>!(odd X or even Y) -> true, false otherwise</returns>
        private static bool LeftClickTest(int x, int y)
        {
            return !(((x & 1) == 1 || (y & 1) != 1) ^ ((x & 1) != 1 || (y & 1) == 1));
        }

        /// <summary>
        /// Test for right click dithering.
        /// </summary>
        /// <param name="x">X component of coordinate.</param>
        /// <param name="y">Y component of coordinate.</param>
        /// <returns>!(odd + odd ^ even + even) -> true, false otherwise</returns>
        private static bool RightClickTest(int x, int y)
        {
            return !(((x & 1) == 1 && (y & 1) == 1) ^ ((x & 1) != 1 && (y & 1) != 1));
        }

        /// <summary>
        /// Draws the pattern on the clicked pixels.
        /// </summary>
        /// <param name="e"></param>
        private void Draw(MouseEventArgs e)
        {
            SaveLayerMemento(true);

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetPixelWithThickness(_drawSurface, p.X, p.Y, ToolColor, Thickness, LeftClickTest);
            }
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                SetPixelWithThickness(_drawSurface, p.X, p.Y, ToolColor, Thickness, RightClickTest);
            }

            if (MirrorMode != MirrorModeStates.OFF)
            {
                p = Mirr(p);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    SetPixelWithThickness(_drawSurface, p.X, p.Y, ToolColor, Thickness, LeftClickTest);
                }
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    SetPixelWithThickness(_drawSurface, p.X, p.Y, ToolColor, Thickness, RightClickTest);
                }
            }

            OnRaiseToolEvent(new ToolEventArgs());
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
            {
                Draw(e);
            }
        }

        /// <summary>
        /// Draws the checkboard pattern with applied thickness.
        /// </summary>
        /// <param name="bmp">Bitmap to draw on.</param>
        /// <param name="x0">X component of coordinate.</param>
        /// <param name="y0">Y component of coordinate.</param>
        /// <param name="c">Color to draw with.</param>
        /// <param name="thickness">Thickness to use.</param>
        public static void SetPixelWithThickness(WriteableBitmap bmp, int x0, int y0, MColor c, ToolThickness thickness, DitheringTest testMethod)
        {
            if (x0 >= 0 && y0 >= 0 && x0 < bmp.PixelWidth && y0 < bmp.PixelHeight && testMethod(x0, y0))
                bmp.SetPixel(x0, y0, c);

            if (thickness > ToolThickness.NORMAL)
            {
                if ((x0 - 1) >= 0 && y0 >= 0 && (x0 - 1) < bmp.PixelWidth && y0 < bmp.PixelHeight && testMethod(x0 - 1, y0))
                    bmp.SetPixel(x0 - 1, y0, c);
                if ((x0 - 1) >= 0 && (y0 - 1) >= 0 && (x0 - 1) < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight && testMethod(x0 - 1, y0 - 1))
                    bmp.SetPixel(x0 - 1, y0 - 1, c);
                if (x0 >= 0 && (y0 - 1) >= 0 && x0 < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight && testMethod(x0, y0 - 1))
                    bmp.SetPixel(x0, y0 - 1, c);

                if (thickness > ToolThickness.MEDIUM)
                {
                    if ((x0 + 1) >= 0 && (y0 + 1) >= 0 && (x0 + 1) < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight && testMethod(x0 + 1, y0 + 1))
                        bmp.SetPixel(x0 + 1, y0 + 1, c);
                    if ((x0 + 1) >= 0 && (y0 - 1) >= 0 && (x0 + 1) < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight && testMethod(x0 + 1, y0 - 1))
                        bmp.SetPixel(x0 + 1, y0 - 1, c);
                    if ((x0 - 1) >= 0 && (y0 + 1) >= 0 && (x0 - 1) < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight && testMethod(x0 - 1, y0 + 1))
                        bmp.SetPixel(x0 - 1, y0 + 1, c);
                    if ((x0 + 1) >= 0 && y0 >= 0 && (x0 + 1) < bmp.PixelWidth && y0 < bmp.PixelHeight && testMethod(x0 + 1, y0))
                        bmp.SetPixel(x0 + 1, y0, c);
                    if (x0 >= 0 && (y0 + 1) >= 0 && x0 < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight && testMethod(x0, y0 + 1))
                        bmp.SetPixel(x0, y0 + 1, c);
                }
            }
        }
    }
}