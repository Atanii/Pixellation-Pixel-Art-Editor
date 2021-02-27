using Pixellation.Components.Editor;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Pixellation.Interfaces
{
    public interface IFrameProvider
    {
        public List<DrawingFrame> Frames { get; }
        public List<DrawingLayer> Layers { get; }
        
        public int ActiveFrameIndex { get; }

        public int ActiveLayerIndex { get; }

        public DrawingLayer ActiveLayer { get; }

        public DrawingFrame ActiveFrame { get; }

        public IEnumerable<BitmapSource> GetFramesAsWriteableBitmaps();
    }
}