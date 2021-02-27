using System.Windows.Media.Imaging;

namespace Pixellation.Interfaces
{
    public interface IBitmapProvider
    {
        public WriteableBitmap Bitmap { get; }

        public bool Visible { get; }

        public double Opacity { get; }
    }
}