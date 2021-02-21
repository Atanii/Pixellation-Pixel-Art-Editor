using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Interfaces
{
    public interface IPreviewable
    {
        public event EventHandler RaiseImageUpdatedEvent;

        public WriteableBitmap GetWriteableBitmap();

        public ImageSource GetImageSource();
    }
}