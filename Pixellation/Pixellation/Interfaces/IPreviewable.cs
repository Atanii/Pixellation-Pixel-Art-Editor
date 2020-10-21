using System;
using System.Windows.Media.Imaging;

namespace Pixellation.Components
{
    public interface IPreviewable
    {
        public event EventHandler RaiseImageUpdatedEvent;

        public WriteableBitmap GetBitmap();
    }
}