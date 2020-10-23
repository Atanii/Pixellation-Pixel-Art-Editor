using Pixellation.Utils;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public class DrawingSurface : FrameworkElement
    {
        protected readonly PixelEditor _owner;
        protected readonly WriteableBitmap _bitmap;

        public DrawingSurface(PixelEditor owner)
        {
            _owner = owner;
            _bitmap = BitmapFactory.New(owner.PixelWidth, owner.PixelHeight);
            _bitmap.Clear(Colors.Transparent);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }

        public DrawingSurface(PixelEditor owner, WriteableBitmap bitmap)
        {
            _owner = owner;
            _bitmap = bitmap;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var magnification = _owner.Magnification;
            var width = _bitmap.PixelWidth * magnification;
            var height = _bitmap.PixelHeight * magnification;

            dc.DrawImage(_bitmap, new Rect(0, 0, width, height));
        }

        internal void SetColor(int x, int y, Color color)
        {
            _bitmap.SetPixel(x, y, color);
        }

        internal Color GetColor(int x, int y)
        {
            return _bitmap.GetPixel(x, y);
        }

        public WriteableBitmap GetWriteableBitmap()
        {
            return this._bitmap;
        }

        public System.Drawing.Bitmap GetBitmap()
        {
            return this._bitmap.ToBitmap();
        }
    }
}