using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Pixellation.Utils
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts a <see cref="WriteableBitmap"/> to a <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="wb">Caller <see cref="WriteableBitmap"/>.</param>
        /// <returns>Resulting <see cref="BitmapImage"/>.</returns>
        public static BitmapImage ToImageSource(this WriteableBitmap wb)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(wb));
                encoder.Save(stream);

                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        /// <summary>
        /// Converts a <see cref="WriteableBitmap"/> to a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="wb">Caller <see cref="WriteableBitmap"/>.</param>
        /// <returns>Resulting <see cref="Bitmap"/>.</returns>
        public static Bitmap ToBitmap(this WriteableBitmap wb)
        {
            using MemoryStream outStream = new MemoryStream();

            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(wb));
            enc.Save(outStream);

            return new Bitmap(outStream);
        }

        /// <summary>
        /// Converts a <see cref="Bitmap"/> to a <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="bitmap">Caller <see cref="Bitmap"/>.</param>
        /// <returns>Resulting <see cref="BitmapImage"/>.</returns>
        public static BitmapImage ToImageSource(this Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {   
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        /// <summary>
        /// Converts a <see cref="Bitmap"/> to a <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="bitmap">Caller <see cref="Bitmap"/>.</param>
        /// <returns>Resulting <see cref="BitmapSource"/>.</returns>
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return source;
        }

        /// <summary>
        /// Converts a <see cref="Bitmap"/> to a <see cref="WriteableBitmap"/>.
        /// </summary>
        /// <param name="bitmap">Caller <see cref="Bitmap"/>.</param>
        /// <returns>Resulting <see cref="WriteableBitmap"/>.</returns>
        public static WriteableBitmap ToWriteableBitmap(this Bitmap bitmap)
        {
            return new WriteableBitmap(bitmap.ToBitmapSource());
        }

        /// <summary>
        /// Converts a <see cref="BitmapSource"/> to a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bmps">Caller <see cref="BitmapSource"/>.</param>
        /// <returns>Resulting <see cref="Bitmap"/>.</returns>
        public static Bitmap ToBitmap(this BitmapSource bmps)
        {
            int width = bmps.PixelWidth;
            int height = bmps.PixelHeight;
            int stride = width * ((bmps.Format.BitsPerPixel + 7) / 8);

            IntPtr ptr = IntPtr.Zero;

            try
            {
                ptr = Marshal.AllocHGlobal(height * stride);
                bmps.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);

                using (var btm = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, ptr))
                {
                    // Clone the bitmap so that we can dispose it and
                    // release the unmanaged memory at ptr
                    return new Bitmap(btm);
                }
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Converts a <see cref="Color"/> to a <see cref="System.Windows.Media.Color"/>.
        /// </summary>
        /// <param name="color">Caller <see cref="Color"/>.</param>
        /// <returns>Resulting <see cref="System.Windows.Media.Color"/>.</returns>
        public static System.Windows.Media.Color ToMediaColor(this Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts a <see cref="System.Windows.Media.Color"/> to a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">Caller <see cref="System.Windows.Media.Color"/>.</param>
        /// <returns>Resulting <see cref="Color"/>.</returns>
        public static Color ToDrawingColor(this System.Windows.Media.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts a <see cref="byte[]"/> to a <see cref="WriteableBitmap"/>.
        /// </summary>
        /// <param name="bytes">Caller <see cref="byte[]"/>.</param>
        /// <param name="width">Width of the bitmap in pixels.</param>
        /// <param name="height">Height of the bitmap in pixels.</param>
        /// <param name="stride">Scan width. Width of a single row of pixels in the bitmap.</param>
        /// <returns>Resulting <see cref="WriteableBitmap"/>.</returns>
        public static WriteableBitmap ToWriteableBitmap(this byte[] bytes, int width, int height, int stride)
        {
            var wrBmp = BitmapFactory.New(width, height);

            wrBmp.Clear(System.Windows.Media.Colors.Transparent);
            wrBmp.WritePixels(
                new Int32Rect(0, 0, width, height),
                bytes,
                stride,
                0
            );

            return wrBmp;
        }

        /// <summary>
        /// Divides the X and the Y component of a <see cref="System.Windows.Point"/> by an <see cref="int"/> divisor.
        /// </summary>
        /// <param name="p">Caller <see cref="System.Windows.Point"/>.</param>
        /// <param name="divisor">An <see cref="int"/> divisor.</param>
        /// <returns>Divided <see cref="System.Windows.Point"/>.</returns>
        public static System.Windows.Point IntDivide(this System.Windows.Point p, int divisor)
        {
            p.X /= divisor;
            p.Y /= divisor;

            return p;
        }

        /// <summary>
        /// Clears all of the pixels contained in- or outside of the given <see cref="GraphicsPath"/>.
        /// </summary>
        /// <param name="bitmap">Caller <see cref="WriteableBitmap"/>.</param>
        /// <param name="path">The <see cref="GraphicsPath"/> for the in/exclusive pixel clearing.</param>
        /// <param name="inThePath">Should only the pixels contained in the path or only the ones outside of the path cleared?</param>
        /// <returns>Cleared <see cref="WriteableBitmap"/>.</returns>
        public static WriteableBitmap ClearPixelsByGraphicsPath(this WriteableBitmap bitmap, GraphicsPath path, bool inThePath = true)
        {
            for (int y = 0; y < bitmap.PixelHeight; y++)
            {
                for (int x = 0; x < bitmap.PixelWidth; x++)
                {
                    if (inThePath && path.IsVisible(x, y))
                    {
                        bitmap.SetPixel(x, y, 0, 0, 0, 0);
                    }
                    else if (!inThePath && !path.IsVisible(x, y))
                    {
                        bitmap.SetPixel(x, y, 0, 0, 0, 0);
                    }
                }
            }
            return bitmap;
        }
    }
}