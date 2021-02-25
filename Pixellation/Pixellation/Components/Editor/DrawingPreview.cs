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

        private int _fps = 24;
        public int FPS
        {
            get => _fps;
            set
            {
                _fps = Math.Clamp(value, 1, 60);
            }
        }
        
        private long TimeBetweenFrames => 1000 / FPS;
        private long MillisecondsNow => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        private long lastTime = 0;

        private int _indexOfCurrentFrame = 0;
        public int IndexOfCurrentFrame
        {
            get => _indexOfCurrentFrame;
            set
            {
                if (FrameProvider != null)
                {
                    var tmp = value % FrameProvider.Frames.Count;
                    if (tmp == 0 && PMode == PreviewMode.PLAY)
                    {
                        PMode = PreviewMode.LAYERS;
                    }
                    _indexOfCurrentFrame = tmp;
                }
            }
        }

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

            DrawingLayer.OnUpdated += () => { InvalidateVisual(); };
            DrawingFrame.OnUpdated += () => { InvalidateVisual(); };
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (FrameProvider != null)
            {
                switch (PMode)
                {
                    // Only layers of selected frame
                    case PreviewMode.LAYERS:
                        RenderLayers(dc);
                        break;

                    // All frames in one image
                    case PreviewMode.FRAMES:
                        RenderFrames(dc);
                        break;

                    // Cycle through available frames
                    case PreviewMode.PLAY:
                    case PreviewMode.LOOP:
                        AnimateFrames(dc);
                        break;

                    default:
                        break;
                }
            }
        }

        private void AnimateFrames(DrawingContext dc)
        {
            // Calls the overwritten OnRender method of DrawingFrame (which renders the layers of drawingframe in one image)
            FrameProvider.Frames[IndexOfCurrentFrame].Render(dc, 0, 0, Width, Height);

            // 24 FPS, 1000ms / 24 time between frames
            if ((MillisecondsNow - lastTime) >= TimeBetweenFrames)
            {
                // Property, set PreviewMode to LAYERS from PLAY after one full run
                // In case of LOOP only if the PreviewMode was set to something else with a button
                IndexOfCurrentFrame += 1;
                lastTime = MillisecondsNow;
            }

            // Call OnRender
            InvalidateVisual();
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