using Pixellation.Components.Editor;
using System.Collections.Generic;

namespace Pixellation.Interfaces
{
    public interface IFrameManager
    {
        public int GetActiveLayerIndex();

        public int GetActiveFrameIndex();

        public DrawingFrame GetActiveDrawingFrame();

        public void MoveDrawingFrameLeft(int frameIndex);

        public void MoveDrawingFrameRight(int frameIndex);

        public void DuplicateDrawingFrame(int frameIndex);

        public void RemoveDrawingFrame(int frameIndex);

        public void AddDrawingFrame(int frameIndex, string name);

        public void MergeDrawingFrameIntoLeftNeighbour(int frameIndex);

        public List<DrawingFrame> GetFrames();
    }
}