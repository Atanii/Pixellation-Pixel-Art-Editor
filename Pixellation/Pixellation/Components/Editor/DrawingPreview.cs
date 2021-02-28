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

        public IFrameProvider DrawingFrameProvider
        {
            get { return (IFrameProvider)GetValue(DrawingFrameProviderProperty); }
            set { SetValue(DrawingFrameProviderProperty, value); }
        }
        public static readonly DependencyProperty DrawingFrameProviderProperty =
         DependencyProperty.Register("DrawingFrameProvider", typeof(IFrameProvider), typeof(DrawingPreview), new FrameworkPropertyMetadata(
             default,
             (s, e) => { IFrameProviderUpdated?.Invoke(s, e); }
        ));

        public PreviewMode PMode
        {
            get { return (PreviewMode)GetValue(PModeProperty); }
            set { SetValue(PModeProperty, value); }
        }
        public static readonly DependencyProperty PModeProperty =
         DependencyProperty.Register("PMode", typeof(PreviewMode), typeof(DrawingPreview), new FrameworkPropertyMetadata(
             PreviewMode.ALL,
             (s, e) => { ModeUpdated?.Invoke(s, e); }
        ));

        private bool _onionModeEnabled;
        public bool OnionModeEnabled
        {
            get => _onionModeEnabled;
            set
            {
                _onionModeEnabled = value;
                InvalidateVisual();
            }
        }

        public DrawingPreview()
        {
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            OnionModeEnabled = false;

            PixelEditor.FrameListChanged += (s, a) => InvalidateVisual();
            PixelEditor.LayerListChanged += (s, a) => InvalidateVisual();

            PixelEditor.RaiseImageUpdatedEvent += (s, a) => InvalidateVisual();

            DrawingLayer.OnUpdated += () => InvalidateVisual();
            DrawingFrame.OnUpdated += () => InvalidateVisual();

            ModeUpdated += (s, e) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (DrawingFrameProvider != null)
            {
                switch (PMode)
                {
                    // Only active layer
                    case PreviewMode.LAYER:
                        RenderLayer(dc);
                        break;

                    // Only layers of selected frame
                    case PreviewMode.FRAME:
                        RenderFrame(dc);
                        break;

                    // All frames in one image
                    case PreviewMode.ALL:
                        RenderFrames(dc);
                        break;

                    default:
                        break;
                }
            }
        }

        private void RenderLayer(DrawingContext dc)
        {
            var index = DrawingFrameProvider.ActiveLayerIndex;

            if (OnionModeEnabled)
            {
                if (DrawingFrameProvider.Layers.Count > (index + 1))
                {
                    DrawingFrameProvider.ActiveFrame.Render(dc, 0, 0, Width, Height, opacity: 0.5f, layerIndex: index + 1);
                }
            }

            DrawingFrameProvider.ActiveFrame.Render(dc, 0, 0, Width, Height, layerIndex: index);
        }

        private void RenderFrame(DrawingContext dc)
        {
            if (OnionModeEnabled)
            {
                var index = DrawingFrameProvider.ActiveFrameIndex;
                if (index > 0)
                {
                    DrawingFrameProvider.Frames[index - 1].Render(dc, 0, 0, Width, Height, opacity: 0.5f);
                }
            }

            DrawingFrameProvider.ActiveFrame.Render(dc, 0, 0, Width, Height);
        }

        private void RenderFrames(DrawingContext dc)
        {
            foreach (var frame in DrawingFrameProvider.Frames)
            {
                frame.Render(dc, 0, 0, Width, Height);
            }
        }
    }
}