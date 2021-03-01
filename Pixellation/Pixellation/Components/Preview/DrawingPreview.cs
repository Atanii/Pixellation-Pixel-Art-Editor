using Pixellation.Components.Editor;
using Pixellation.Interfaces;
using System;
using System.Windows;
using System.Windows.Media;

namespace Pixellation.Components.Preview
{
    /// <summary>
    /// Class for previewing the current project in multiple possible modes based on <see cref="PreviewMode"/>.
    /// </summary>
    internal class DrawingPreview : FrameworkElement
    {
        /// <summary>
        /// Event for signaling the initial or new value set to <see cref="DrawingFrameProvider"/>.
        /// </summary>
        private static event EventHandler<DependencyPropertyChangedEventArgs> IFrameProviderUpdated;

        /// <summary>
        /// Event for signaling a change in the currently set <see cref="PreviewMode"/>.
        /// </summary>
        private static event EventHandler<DependencyPropertyChangedEventArgs> ModeUpdated;

        /// <summary>
        /// Provides frames, indexes, functionality for handling frames.
        /// </summary>
        public IFrameProvider DrawingFrameProvider
        {
            get { return (IFrameProvider)GetValue(DrawingFrameProviderProperty); }
            set { SetValue(DrawingFrameProviderProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="DrawingFrameProvider"/>.
        /// </summary>
        public static readonly DependencyProperty DrawingFrameProviderProperty =
         DependencyProperty.Register("DrawingFrameProvider", typeof(IFrameProvider), typeof(DrawingPreview), new FrameworkPropertyMetadata(
             default,
             (s, e) => { IFrameProviderUpdated?.Invoke(s, e); }
        ));

        /// <summary>
        /// Mode for rendering.
        /// </summary>
        public PreviewMode PMode
        {
            get { return (PreviewMode)GetValue(PModeProperty); }
            set { SetValue(PModeProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="PMode"/>.
        /// </summary>
        public static readonly DependencyProperty PModeProperty =
         DependencyProperty.Register("PMode", typeof(PreviewMode), typeof(DrawingPreview), new FrameworkPropertyMetadata(
             PreviewMode.ALL,
             (s, e) => { ModeUpdated?.Invoke(s, e); }
        ));

        private bool _onionModeEnabled;

        /// <summary>
        /// Indicates if onion mode is enabled.
        /// </summary>
        public bool OnionModeEnabled
        {
            get => _onionModeEnabled;
            set
            {
                _onionModeEnabled = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Sets event handling, bitmapscaling and default values.
        /// </summary>
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

        /// <summary>
        /// Depending on which <see cref="PreviewMode"/> is set, it renders the selected layer, frame or the whole edited image (all frames).
        /// In case of onion mode it renders the one behind the selected with lower opacity for helping animation frame drawing.
        /// </summary>
        /// <param name="dc"></param>
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

        /// <summary>
        /// Renders only the selected layer (and the one behind it in onion mode).
        /// </summary>
        /// <param name="dc"></param>
        private void RenderLayer(DrawingContext dc)
        {
            var index = DrawingFrameProvider.ActiveLayerIndex;

            // Renders the one behind the selected with lower opacity.
            if (OnionModeEnabled)
            {
                if (DrawingFrameProvider.Layers.Count > (index + 1))
                {
                    DrawingFrameProvider.ActiveFrame.Render(dc, 0, 0, Width, Height, opacity: 0.5f, layerIndex: index + 1);
                }
            }

            DrawingFrameProvider.ActiveFrame.Render(dc, 0, 0, Width, Height, layerIndex: index);
        }

        /// <summary>
        /// Renders only the selected frame (and the one behind it in onion mode).
        /// </summary>
        /// <param name="dc"></param>
        private void RenderFrame(DrawingContext dc)
        {
            // Renders the one behind the selected with lower opacity.
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

        /// <summary>
        /// Render all frames on top of each other.
        /// </summary>
        /// <param name="dc"></param>
        private void RenderFrames(DrawingContext dc)
        {
            foreach (var frame in DrawingFrameProvider.Frames)
            {
                frame.Render(dc, 0, 0, Width, Height);
            }
        }
    }
}