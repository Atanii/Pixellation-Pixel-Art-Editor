using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Tool for lightening and darkening pixels.
    /// </summary>
    public class ShadingTool : BaseMultitonTool<ShadingTool>
    {
        private readonly Cursor _cursor = GetCursorFromResource("cursor-darkenlighten.cur");
        public override Cursor ToolCursor { get => _cursor; }

        public override bool EraserModeCompatible => false;

        private ShadingTool() : base()
        {
        }

        /// <summary>
        /// Lightening or darkening the clicked pixels.
        /// </summary>
        /// <param name="e"></param>
        private void Draw(MouseEventArgs e)
        {
            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            if (OutOfBounds(p) || _drawSurface.GetPixel(p.X, p.Y).A == 0)
            {
                return;
            }

            SaveLayerMemento(true);

            float modifier = 0.01f;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                modifier *= -1;
            }

            DarkenOrLightenPixelWithThickness(_drawSurface, p.X, p.Y, Thickness, modifier);
            if (MirrorMode != MirrorModeStates.OFF)
            {
                p = Mirr(p);
                DarkenOrLightenPixelWithThickness(_drawSurface, p.X, p.Y, Thickness, modifier);
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
                Draw(e);
        }

        /// <summary>
        /// Darken of lighten one unit of draw area (default is 1px).
        /// Based on the given thickness it can affect the surrounding pixels making a bigger draw "unit".
        /// </summary>
        /// <param name="bmp">Bitmap to draw on.</param>
        /// <param name="x0">X component of coordinate.</param>
        /// <param name="y0">Y component of coordinate.</param>
        /// <param name="thickness">Thickness to use.</param>
        /// <param name="modifier">Value to darken or lighten with.</param>
        public static void DarkenOrLightenPixelWithThickness(WriteableBitmap bmp, int x0, int y0, ToolThickness thickness, float modifier)
        {
            if (x0 >= 0 && y0 >= 0 && x0 < bmp.PixelWidth && y0 < bmp.PixelHeight)
            {
                bmp.SetPixel(x0, y0, ColorUtils.ModifiyLightness(bmp.GetPixel(x0, y0), modifier));
            }

            if (thickness > ToolThickness.NORMAL)
            {
                if ((x0 - 1) >= 0 && y0 >= 0 && (x0 - 1) < bmp.PixelWidth && y0 < bmp.PixelHeight)
                {
                    bmp.SetPixel(x0 - 1, y0, ColorUtils.ModifiyLightness(bmp.GetPixel(x0 - 1, y0), modifier));
                }
                if ((x0 - 1) >= 0 && (y0 - 1) >= 0 && (x0 - 1) < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                {
                    bmp.SetPixel(x0 - 1, y0 - 1, ColorUtils.ModifiyLightness(bmp.GetPixel(x0 - 1, y0 - 1), modifier));
                }
                if (x0 >= 0 && (y0 - 1) >= 0 && x0 < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                {
                    bmp.SetPixel(x0, y0 - 1, ColorUtils.ModifiyLightness(bmp.GetPixel(x0, y0 - 1), modifier));
                }

                if (thickness > ToolThickness.MEDIUM)
                {
                    if ((x0 + 1) >= 0 && (y0 + 1) >= 0 && (x0 + 1) < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                    {
                        bmp.SetPixel(x0 + 1, y0 + 1, ColorUtils.ModifiyLightness(bmp.GetPixel(x0 + 1, y0 + 1), modifier));
                    }
                    if ((x0 + 1) >= 0 && (y0 - 1) >= 0 && (x0 + 1) < bmp.PixelWidth && (y0 - 1) < bmp.PixelHeight)
                    {
                        bmp.SetPixel(x0 + 1, y0 - 1, ColorUtils.ModifiyLightness(bmp.GetPixel(x0 + 1, y0 - 1), modifier));
                    }
                    if ((x0 - 1) >= 0 && (y0 + 1) >= 0 && (x0 - 1) < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                    {
                        bmp.SetPixel(x0 - 1, y0 + 1, ColorUtils.ModifiyLightness(bmp.GetPixel(x0 - 1, y0 + 1), modifier));
                    }
                    if ((x0 + 1) >= 0 && y0 >= 0 && (x0 + 1) < bmp.PixelWidth && y0 < bmp.PixelHeight)
                    {
                        bmp.SetPixel(x0 + 1, y0, ColorUtils.ModifiyLightness(bmp.GetPixel(x0 + 1, y0), modifier));
                    }
                    if (x0 >= 0 && (y0 + 1) >= 0 && x0 < bmp.PixelWidth && (y0 + 1) < bmp.PixelHeight)
                    {
                        bmp.SetPixel(x0, y0 + 1, ColorUtils.ModifiyLightness(bmp.GetPixel(x0, y0 + 1), modifier));
                    }
                }
            }
        }
    }
}