using Pixellation.Components.Editor;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Preview
{
    /// <summary>
    /// Simple preview class for previewing one bitmap.
    /// </summary>
    internal class BitmapPreview : FrameworkElement
    {
        /// <summary>
        /// Property for bitmap is set initially or updated.
        /// </summary>
        private static event EventHandler<DependencyPropertyChangedEventArgs> BmpPropertyChanged;

        /// <summary>
        /// Bitmap to render.
        /// </summary>
        public WriteableBitmap Bmp
        {
            get { return (WriteableBitmap)GetValue(BmpProperty); }
            set { SetValue(BmpProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the bitmap.
        /// </summary>
        public static readonly DependencyProperty BmpProperty =
         DependencyProperty.Register("Bmp", typeof(WriteableBitmap), typeof(BitmapPreview), new FrameworkPropertyMetadata(
             default,
             (s, e) => { BmpPropertyChanged?.Invoke(s, e); }
        ));

        /// <summary>
        /// Sets event handling.
        /// </summary>
        public BitmapPreview()
        {
            // Due to operations like layer merging.
            PixelEditor.LayerListChanged += (a) => InvalidateVisual();
            // Drawing.
            PixelEditor.RaiseImageUpdatedEvent += InvalidateVisual;
            // New bitmap set.
            DrawingLayer.PropertyUpdated += () => InvalidateVisual();
        }

        /// <summary>
        /// Renders <see cref="Bmp"/> centered on this element.
        /// </summary>
        /// <param name="drawingContext">Context for drawing.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Not rendering anything if not bitmap is provided (yet).
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