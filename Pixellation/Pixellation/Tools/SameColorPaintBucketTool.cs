using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    /// <summary>
    /// Paint bucket tool coloring every pixel with the same color as the clicked one with the currently set color.
    /// </summary>
    public class SameColorPaintBucketTool : BaseMultitonTool<SameColorPaintBucketTool>
    {
        private SameColorPaintBucketTool() : base()
        {
        }

        /// <summary>
        /// Gets the clicked pixel and start coloring.
        /// </summary>
        /// <param name="e"></param>
        private void Draw(MouseButtonEventArgs e)
        {
            SaveLayerMemento();

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            var targetColor = _drawSurface.GetPixel(p.X, p.Y);
            var replacementColor = ToolColor;

            ColorPixels(targetColor, replacementColor);
        }

        /// <summary>
        /// Coloring every pixel with the target color with the replacement color.
        /// </summary>
        /// <param name="targetColor">Color to replace.</param>
        /// <param name="replacementColor">Color with replace.</param>
        private void ColorPixels(System.Windows.Media.Color targetColor, System.Windows.Media.Color replacementColor)
        {
            for (int x = 0; x < _drawSurface.PixelWidth; x++)
            {
                for (int y = 0; y < _drawSurface.PixelHeight; y++)
                {
                    if (_drawSurface.GetPixel(x, y) == targetColor)
                    {
                        _drawSurface.SetPixel(x, y, replacementColor);
                    }
                }
            }
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            Draw(e);
        }
    }
}