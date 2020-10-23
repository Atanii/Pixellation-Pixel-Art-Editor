using System;
using System.Windows.Media.Imaging;

namespace Pixellation.Components
{
    public interface IPreviewable
    {
        public event EventHandler RaiseImageUpdatedEvent;

        public WriteableBitmap GetWriteableBitmap();

        public BitmapImage GetImageSource(int width = 0, int height = 0);
    }
}