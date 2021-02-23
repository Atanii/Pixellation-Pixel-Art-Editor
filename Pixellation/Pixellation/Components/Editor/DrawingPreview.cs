using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Pixellation.Components.Editor
{
    internal class DrawingPreview : FrameworkElement
    {
        public List<DrawingFrame> Frames
        {
            get { return (List<DrawingFrame>)GetValue(FramesProperty); }
            set { SetValue(FramesProperty, value); }
        }

        public static readonly DependencyProperty FramesProperty =
         DependencyProperty.Register("Frames", typeof(List<DrawingFrame>), typeof(DrawingPreview), new FrameworkPropertyMetadata(
             new List<DrawingFrame>()
        ));

        public List<DrawingLayer> Layers
        {
            get { return (List<DrawingLayer>)GetValue(LayersProperty); }
            set { SetValue(LayersProperty, value); }
        }

        public static readonly DependencyProperty LayersProperty =
         DependencyProperty.Register("Layers", typeof(List<DrawingLayer>), typeof(DrawingPreview), new FrameworkPropertyMetadata(
             new List<DrawingLayer>()
        ));

        public DrawingPreview()
        {
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            PixelEditor.RaiseImageUpdatedEvent += (s, a) =>
            {
                InvalidateVisual();
            };
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (Frames.Count > 0)
            {
                RenderFrames(dc);
            }
            else
            {
                RenderLayers(dc);
            }
        }

        private void RenderLayers(DrawingContext dc)
        {
            foreach (var layer in Layers)
            {
                layer.Render(dc, 0, 0, Width, Height);
            }
        }

        private void RenderFrames(DrawingContext dc)
        {
            foreach (var frame in Frames)
            {
                frame.Render(dc, 0, 0, Width, Height);
            }
        }
    }
}