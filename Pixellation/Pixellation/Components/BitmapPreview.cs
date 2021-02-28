using Pixellation.Components.Editor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components
{
    class BitmapPreview : FrameworkElement
    {
        private static event EventHandler<DependencyPropertyChangedEventArgs> BmpPropertyChanged;

        public WriteableBitmap Bmp
        {
            get { return (WriteableBitmap)GetValue(BmpProperty); }
            set { SetValue(BmpProperty, value); }
        }

        public static readonly DependencyProperty BmpProperty =
         DependencyProperty.Register("Bmp", typeof(WriteableBitmap), typeof(BitmapPreview), new FrameworkPropertyMetadata(
             default,
             (s, e) => { BmpPropertyChanged?.Invoke(s, e); }
        ));

        public BitmapPreview()
        {
            PixelEditor.RaiseImageUpdatedEvent += (s, e) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (Bmp == null)
            {
                return;
            }

            // Calculate size and position according to the current magnification.
            var bmpw = Bmp.PixelWidth;
            var bmph = Bmp.PixelHeight;

            double w, h;
            if (bmpw >= bmph && bmpw > Width)
            {
                w = Width;
                h = (w / bmpw) * bmph;
            }
            else if (bmph >= bmpw && bmph > Height)
            {
                h = Height;
                w = (h / bmph) * bmpw;
            }
            else
            {
                w = bmpw;
                h = bmph;
            }
            double x = (Width - w) / 2;
            double y = (Height - h) / 2;

            drawingContext.DrawImage(Bmp, new Rect(x, y, w, h));
        }
    }
}
