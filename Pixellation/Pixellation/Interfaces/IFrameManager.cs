using Pixellation.Components.Editor;
using System.Collections.Generic;

namespace Pixellation.Interfaces
{
    public interface IFrameManager
    {
        public int ActiveLayerIndex { get; }

        public int ActiveFrameIndex { get; }

        public void MoveDrawingFrameLeft(int frameIndex);

        public void MoveDrawingFrameRight(int frameIndex);

        public void DuplicateDrawingFrame(int frameIndex);

        public void RemoveDrawingFrame(int frameIndex, bool removeCaretakerAsWell = false);

        public void ResetDrawingFrame(int frameIndex);

        public void AddDrawingFrame(int frameIndex, string name);

        public void MergeDrawingFrameIntoLeftNeighbour(int frameIndex);

        public List<DrawingFrame> Frames { get; }

        public void UndoFrameOperation();

        public void RedoFrameOperation();

        public void SaveState(int eTypeValue, int frameIndex);
    }
}