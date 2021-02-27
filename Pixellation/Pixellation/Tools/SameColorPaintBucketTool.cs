using Pixellation.Utils;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    public class SameColorPaintBucketTool : BaseMultitonTool<SameColorPaintBucketTool>
    {
        private SameColorPaintBucketTool() : base()
        {
        }

        private void Draw(MouseButtonEventArgs e)
        {
            SaveLayerMemento();

            var p = e.GetPosition(_layer).DivideByIntAsIntPoint(_magnification);

            var targetColor = _layer.GetPixel(p.X, p.Y);
            var replacementColor = ToolColor;

            ColorPixels(targetColor, replacementColor);
        }

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