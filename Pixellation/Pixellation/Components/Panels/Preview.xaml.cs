using Pixellation.Utils;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for Preview.xaml
    /// </summary>
    public partial class Preview : UserControl
    {
        public IPreviewable VisualToPreview
        {
            get { return (IPreviewable)GetValue(VisualToPreviewProperty); }
            set { SetValue(VisualToPreviewProperty, value); }
        }

        public static readonly DependencyProperty VisualToPreviewProperty =
         DependencyProperty.Register("VisualToPreview", typeof(IPreviewable), typeof(Preview), new FrameworkPropertyMetadata(
             default,
             (d, e) => { RaiseVisualToPreviewChangeEvent?.Invoke(d, e); }
        ));

        private static event PropertyChangedCallback RaiseVisualToPreviewChangeEvent;

        public Preview()
        {
            InitializeComponent();
            RaiseVisualToPreviewChangeEvent += VisualToPreviewPropertyInitOrUpdated;
        }

        private void VisualToPreviewPropertyInitOrUpdated(object e, DependencyPropertyChangedEventArgs a)
        {
            VisualToPreview.RaiseImageUpdatedEvent += Update;
        }

        private void Update(object sender, EventArgs e)
        {
            // TODO: fix transparency
            var bitmap = VisualToPreview.GetWriteableBitmap().ToBitmap();
            var b = bitmap.ToBitmapSource();
            imgPreview.Source = b;
        }
    }
}