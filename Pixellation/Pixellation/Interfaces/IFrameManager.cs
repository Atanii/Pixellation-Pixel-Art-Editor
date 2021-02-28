using Pixellation.Components.Editor;
using System.Collections.Generic;

namespace Pixellation.Interfaces
{
    /// <summary>
    /// Provieds helping functionality for frame lists and similar components.
    /// </summary>
    public interface IFrameManager
    {
        /// <summary>
        /// Index of selected layer.
        /// </summary>
        public int ActiveLayerIndex { get; }

        /// <summary>
        /// Index of selected frame.
        /// </summary>
        public int ActiveFrameIndex { get; }

        /// <summary>
        /// Move frame to the left (behind).
        /// </summary>
        /// <param name="frameIndex">Index of frame to move.</param>
        public void MoveDrawingFrameLeft(int frameIndex);

        /// <summary>
        /// Moves frame to the right (closer).
        /// </summary>
        /// <param name="frameIndex">Index of frame to move.</param>
        public void MoveDrawingFrameRight(int frameIndex);

        /// <summary>
        /// Duplicates a frame.
        /// </summary>
        /// <param name="frameIndex">Index of frame to duplicate.</param>
        public void DuplicateDrawingFrame(int frameIndex);

        /// <summary>
        /// Removes a frame.
        /// </summary>
        /// <param name="frameIndex">Index of frame to duplicate.</param>
        /// <param name="removeCaretakerAsWell">Remove the undo-redo handler for the layers of this frame?</param>
        public void RemoveDrawingFrame(int frameIndex, bool removeCaretakerAsWell = false);

        /// <summary>
        /// Resets a frame, leaving only one blank layer. Irreversible operation.
        /// </summary>
        /// <param name="frameIndex">Index of frame to reset.</param>
        public void ResetDrawingFrame(int frameIndex);

        /// <summary>
        /// Adds a new frame.
        /// </summary>
        /// <param name="frameIndex">Index of new frame.</param>
        /// <param name="name">Name of the new frame.</param>
        public void AddDrawingFrame(int frameIndex, string name);

        /// <summary>
        /// Merges a frame into the left neighbour. Irreversible operation!
        /// </summary>
        /// <param name="frameIndex">Index of frame to merge.</param>
        public void MergeDrawingFrameIntoLeftNeighbour(int frameIndex);

        /// <summary>
        /// Frames of the current project.
        /// </summary>
        public List<DrawingFrame> Frames { get; }

        /// <summary>
        /// Undo previous operation applied on frames.
        /// </summary>
        public void UndoFrameOperation();

        /// <summary>
        /// Redo previous operation that was undone.
        /// </summary>
        public void RedoFrameOperation();

        /// <summary>
        /// Saves the state of a frame for possible undo.
        /// </summary>
        /// <param name="eTypeValue">Value of <see cref="IPixelEditorEventType"/>.</param>
        /// <param name="frameIndex">Index of frame to save.</param>
        public void SaveState(int eTypeValue, int frameIndex);
    }
}