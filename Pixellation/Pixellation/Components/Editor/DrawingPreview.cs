using Pixellation.Interfaces;
using System;
using System.Windows;
using System.Windows.Media;

namespace Pixellation.Components.Editor
{
    internal class DrawingPreview : FrameworkElement
    {
        private static event EventHandler<DependencyPropertyChangedEventArgs> IFrameProviderUpdated;
        private static event EventHandler<DependencyPropertyChangedEventArgs> ModeUpdated;

        public IFrameProvider FrameProvider
        {
            get { return (IFrameProvider)GetValue(FrameProviderProperty); }
            set { SetValue(FrameProviderProperty, value); }
        }

        public static readonly DependencyProperty FrameProviderProperty =
         DependencyProperty.Register("FrameProvider", typeof(IFrameProvider), typeof(DrawingPreview), new FrameworkPropertyMetadata(
             default,
             (s, e) => { IFrameProviderUpdated?.Invoke(s, e); }
        ));

        public enum PreviewMode {
            LAYERS, FRAMES, ANIMATION
        }

        public PreviewMode PMode
        {
            get { return (PreviewMode)GetValue(PModeProperty); }
            set { SetValue(PModeProperty, value); }
        }

        public static readonly DependencyProperty PModeProperty =
         DependencyProperty.Register("PMode", typeof(PreviewMode), typeof(DrawingPreview), new FrameworkPropertyMetadata(
             PreviewMode.FRAMES,
             (s, e) => { ModeUpdated?.Invoke(s, e); }
        ));

        public DrawingPreview()
        {
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            PixelEditor.FrameListChanged += (s, a) => { InvalidateVisual(); };
            PixelEditor.LayerListChanged += (s, a) => { InvalidateVisual(); };
            PixelEditor.RaiseImageUpdatedEvent += (s, a) => { InvalidateVisual(); };
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (FrameProvider != null)
            {
                switch (PMode)
                {
                    case PreviewMode.LAYERS:
                        RenderLayers(dc);
                        break;

                    case PreviewMode.FRAMES:
                        RenderFrames(dc);
                        break;

                    default:
                        break;
                }
            }
        }

        private void RenderLayers(DrawingContext dc)
        {
            foreach (var layer in FrameProvider.Layers)
            {
                layer.Render(dc, 0, 0, Width, Height);
            }
        }

        private void RenderFrames(DrawingContext dc)
        {
            foreach (var frame in FrameProvider.Frames)
            {
                frame.Render(dc, 0, 0, Width, Height);
            }
        }
    }
}