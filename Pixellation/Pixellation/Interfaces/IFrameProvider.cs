using Pixellation.Components.Editor;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Pixellation.Interfaces
{
    /// <summary>
    /// Provides frame and layers.
    /// </summary>
    public interface IFrameProvider
    {
        /// <summary>
        /// Frames of current project.
        /// </summary>
        public List<DrawingFrame> Frames { get; }

        /// <summary>
        /// Layers of selected frame.
        /// </summary>
        public List<DrawingLayer> Layers { get; }
        
        /// <summary>
        /// Index of selected frame.
        /// </summary>
        public int ActiveFrameIndex { get; }

        /// <summary>
        /// Index of selected layer.
        /// </summary>
        public int ActiveLayerIndex { get; }

        /// <summary>
        /// Selected layer.
        /// </summary>
        public DrawingLayer ActiveLayer { get; }

        /// <summary>
        /// Selected frame.
        /// </summary>
        public DrawingFrame ActiveFrame { get; }

        /// <summary>
        /// Get the selected frame as a merged bitmap.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BitmapSource> GetFramesAsWriteableBitmaps();
    }
}