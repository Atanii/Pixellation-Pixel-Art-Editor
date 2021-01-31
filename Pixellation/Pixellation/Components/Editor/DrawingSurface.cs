using Pixellation.Utils;
using System;
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
            _bitmap = BitmapFactory.New(_owner.PixelWidth, _owner.PixelHeight);
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

            if (_owner.Tiled)
            {
                var temp = (WriteableBitmap)_bitmap.Clone();
                temp.BlitRender(temp, false, _owner.TiledOpacity);
                for (int x = -5 * width; x <= 5 * width; x += width)
                {
                    for (int y = -5 * height; y <= 5 * height; y += height)
                    {
                        if (x == 0 && y == 0)
                            continue;
                        dc.DrawImage(temp, new Rect(x, y, width, height));
                    }
                }
            }
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

        public WriteableBitmap GetWriteableBitmapWithAppliedOpacity()
        {
            var tmp = BitmapFactory.New(_owner.PixelWidth, _owner.PixelHeight);
            tmp.Clear(Colors.Transparent);

            for (int i = 0; i < tmp.Width; i++)
            {
                for (int j = 0; j < tmp.Height; j++)
                {
                    var c = _bitmap.GetPixel(i, j);
                    if (c.A == 0)
                    {
                        continue;
                    }
                    c.A = (byte) Math.Floor(Opacity * 255.0);
                    tmp.SetPixel(i, j, c);
                }
            }

            return tmp;
        }

        public System.Drawing.Bitmap GetBitmap()
        {
            return this._bitmap.ToBitmap();
        }

        public ImageSource GetImageSource()
        {
            return _bitmap.ToImageSource();
        }
    }
}