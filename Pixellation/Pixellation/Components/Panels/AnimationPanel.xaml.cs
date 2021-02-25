using Pixellation.Components.Editor;
using Pixellation.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for AnimationPanel.xaml
    /// </summary>
    public partial class AnimationPanel : UserControl
    {
        private static event EventHandler<DependencyPropertyChangedEventArgs> IFrameProviderUpdated;

        public IFrameProvider AnimationFrameProvider
        {
            get { return (IFrameProvider)GetValue(AnimationFrameProviderProperty); }
            set { SetValue(AnimationFrameProviderProperty, value); }
        }

        public static readonly DependencyProperty AnimationFrameProviderProperty =
         DependencyProperty.Register("AnimationFrameProvider", typeof(IFrameProvider), typeof(AnimationPanel), new FrameworkPropertyMetadata(
             default,
             (s, e) => { IFrameProviderUpdated?.Invoke(s, e); }
        ));

        public AnimationPanel()
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            PixelEditor.FrameListChanged += (s, a) => { InvalidateVisual(); };
            PixelEditor.LayerListChanged += (s, a) => { InvalidateVisual(); };
            PixelEditor.RaiseImageUpdatedEvent += (s, a) => { InvalidateVisual(); };
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            dPreview.PMode = PreviewMode.PLAY;
            dPreview.InvalidateVisual();
        }

        private void Loop(object sender, RoutedEventArgs e)
        {
            dPreview.PMode = PreviewMode.LOOP;
            dPreview.InvalidateVisual();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            dPreview.PMode = PreviewMode.LAYERS;
            dPreview.InvalidateVisual();
        }
    }
}
