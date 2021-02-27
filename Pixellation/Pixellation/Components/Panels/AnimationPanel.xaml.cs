using Pixellation.Components.Editor;
using Pixellation.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for AnimationPanel.xaml
    /// </summary>
    public partial class AnimationPanel : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Event used for one- and twoway databinding.
        /// Marks change regarding one of the properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public IFrameProvider AnimationFrameProvider
        {
            get { return (IFrameProvider)GetValue(AnimationFrameProviderProperty); }
            set { SetValue(AnimationFrameProviderProperty, value); }
        }

        public static readonly DependencyProperty AnimationFrameProviderProperty =
         DependencyProperty.Register("AnimationFrameProvider", typeof(IFrameProvider), typeof(AnimationPanel), new FrameworkPropertyMetadata(
             default
        ));

        public AnimationPanel()
        {
            InitializeComponent();

            player.SetParameters(framePerSecond: Properties.Settings.Default.DefaultAnimationFPS);
            FPSslider.Value = Properties.Settings.Default.DefaultAnimationFPS;

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            PixelEditor.FrameListChanged += (s, a) => { InvalidateVisual(); };
            PixelEditor.LayerListChanged += (s, a) => { InvalidateVisual(); };
            PixelEditor.RaiseImageUpdatedEvent += (s, a) => { InvalidateVisual(); };
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            player.SetParameters(framesToPlay: AnimationFrameProvider.Frames, playOnce: true);
            player.Start();
        }

        private void Loop(object sender, RoutedEventArgs e)
        {
            player.SetParameters(framesToPlay: AnimationFrameProvider.Frames, playOnce: false);
            player.Start();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            player.Stop();
        }

        private void FPSslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.SetParameters(framePerSecond: (int)e.NewValue);
        }

        /// <summary>
        /// Used as change notification for one- and twoway binding with <see cref="DependencyProperty"/> objects.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}