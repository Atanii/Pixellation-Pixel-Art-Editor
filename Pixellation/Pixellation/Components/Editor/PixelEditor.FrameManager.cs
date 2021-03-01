using Pixellation.Components.Event;
using Pixellation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor : IFrameManager
    {
        public static event PixelEditorFrameEventHandler FrameListChanged;

        /// <summary>
        /// Frames of the current project.
        /// </summary>
        public List<DrawingFrame> Frames { get; private set; } = new List<DrawingFrame> { };

        /// <summary>
        /// Unique id of the selected frame.
        /// </summary>
        public string ActiveFrameId => ActiveFrame != null ? ActiveFrame.Id : "";

        private int _activeFrameIndex;

        /// <summary>
        /// Index of selected frame.
        /// </summary>
        public int ActiveFrameIndex
        {
            get { return _activeFrameIndex; }
            private set
            {
                RemoveLayersFromVisualChildren();

                _activeFrameIndex = value;
                _activeFrame = Frames[value];
                _caretaker.ActiveKey = Frames[value].Id;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ActiveFrame));
                OnPropertyChanged(nameof(Frames));

                RefreshVisualsThenSignalUpdate();
            }
        }

        private DrawingFrame _activeFrame;

        /// <summary>
        /// Selected frame.
        /// </summary>
        public DrawingFrame ActiveFrame
        {
            get { return _activeFrame; }
            private set
            {
                RemoveLayersFromVisualChildren();

                _activeFrame = value;
                _activeFrameIndex = Frames.FindIndex(x => x.Id == value.Id);

                OnPropertyChanged();
                OnPropertyChanged(nameof(ActiveFrameIndex));
                OnPropertyChanged(nameof(Frames));

                RefreshVisualsThenSignalUpdate();
            }
        }

        /// <summary>
        /// Get the selected frame as a merged bitmap.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BitmapSource> GetFramesAsWriteableBitmaps()
        {
            var bmps = new List<WriteableBitmap>();
            foreach (var frame in Frames)
            {
                bmps.Add(frame.Bitmap);
            }
            return bmps;
        }

        /// <summary>
        /// Adds a new frame.
        /// </summary>
        /// <param name="frameIndex">Index of new frame.</param>
        /// <param name="name">Name of the new frame.</param>
        public void AddDrawingFrame(int frameIndex, string name)
        {
            if (Frames.Count == 0 && frameIndex == 0)
            {
                var tmp = new DrawingFrame("Default", this);
                _caretaker.InitCaretaker(tmp.Id);
                Frames.Insert(frameIndex, tmp);

                ActiveFrameIndex = 0;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.FRAME_ADD, frameIndex));

                SetActiveLayer();
            }
            else if (Frames.Count >= (frameIndex + 1))
            {
                var tmp = new DrawingFrame(name, this, true, true);
                _caretaker.InitCaretaker(tmp.Id);
                Frames.Insert(frameIndex + 1, tmp);

                ActiveFrameIndex = frameIndex + 1;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.FRAME_ADD, frameIndex + 1));

                SetActiveLayer();
            }
        }

        /// <summary>
        /// Readds a drawing frame from a memento.
        /// </summary>
        /// <param name="frameFromMemento">Memento to restore frame from.</param>
        /// <param name="index">Original index of the frame.</param>
        private void ReAddDrawingFrames(DrawingFrame frameFromMemento, int index)
        {
            Frames.Insert(index, frameFromMemento);

            var tmp = ActiveFrameIndex;
            ActiveFrameIndex = index;

            FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.FRAME_ADD, index));

            SetActiveLayer();

            InvalidateVisual();
        }

        /// <summary>
        /// Adds a drawing frame.
        /// </summary>
        /// <param name="frames">Frame to add.</param>
        private void AddDrawingFrames(List<DrawingFrame> frames)
        {
            foreach (var frame in frames)
            {
                _caretaker.InitCaretaker(frame.Id);
                Frames.Add(frame);
            }

            ActiveFrameIndex = 0;

            FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.FRAME_ADD, 0));

            SetActiveLayer();
        }

        /// <summary>
        /// Duplicates a frame.
        /// </summary>
        /// <param name="frameIndex">Index of frame to duplicate.</param>
        public void DuplicateDrawingFrame(int frameIndex)
        {
            if (Frames.Count >= (frameIndex + 1))
            {
                var tmp = Frames[frameIndex].Clone();
                _caretaker.InitCaretaker(tmp.Id);
                Frames.Insert(frameIndex + 1, tmp);

                ActiveFrameIndex = frameIndex + 1;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.FRAME_DUPLICATE, frameIndex + 1));

                SetActiveLayer();
            }
        }

        /// <summary>
        /// Merges a frame into the left neighbour. Irreversible operation!
        /// </summary>
        /// <param name="frameIndex">Index of frame to merge.</param>
        public void MergeDrawingFrameIntoLeftNeighbour(int frameIndex)
        {
            if (frameIndex > 0 && Frames.Count >= (frameIndex + 1))
            {
                // Get frame to merge.
                var tmp = Frames[frameIndex];

                // Clear undo-redo for layers in the frame the other one will be merged into.
                _caretaker.Clear(Frames[frameIndex - 1].Id);
                // Add new layers to frame to the left.
                Frames[frameIndex - 1].Layers.AddRange(tmp.Layers);

                // Remove undo-redo for frame to be deleted.
                _caretaker.RemoveCaretaker(Frames[frameIndex].Id);
                // Remove merged frame.
                Frames.RemoveAt(frameIndex);

                // Clear undo-redo for frames.
                _caretaker.Clear(FramesCaretakerKey);

                // New active frame is the one the other was merged in.
                ActiveFrameIndex = frameIndex - 1;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.FRAME_MERGE_TO_LEFT, frameIndex - 1));

                SetActiveLayer();
            }
        }

        /// <summary>
        /// Move frame to the left (behind).
        /// </summary>
        /// <param name="frameIndex">Index of frame to move.</param>
        public void MoveDrawingFrameLeft(int frameIndex)
        {
            if (frameIndex > 0 && Frames.Count >= (frameIndex + 1))
            {
                var tmp = Frames[frameIndex];
                Frames.RemoveAt(frameIndex);
                Frames.Insert(frameIndex - 1, tmp);

                ActiveFrameIndex = frameIndex - 1;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.FRAME_MOVE_LEFT, frameIndex - 1));

                SetActiveLayer();
            }
        }

        /// <summary>
        /// Moves frame to the right (closer).
        /// </summary>
        /// <param name="frameIndex">Index of frame to move.</param>
        public void MoveDrawingFrameRight(int frameIndex)
        {
            if (Frames.Count > (frameIndex + 1))
            {
                var tmp = Frames[frameIndex];
                Frames.RemoveAt(frameIndex);
                Frames.Insert(frameIndex + 1, tmp);

                ActiveFrameIndex = frameIndex + 1;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.FRAME_MOVE_RIGHT, frameIndex + 1));

                SetActiveLayer();
            }
        }

        /// <summary>
        /// Removes a frame.
        /// </summary>
        /// <param name="frameIndex">Index of frame to duplicate.</param>
        /// <param name="removeCaretakerAsWell">Remove the undo-redo handler for the layers of this frame?</param>
        public void RemoveDrawingFrame(int frameIndex, bool removeCaretakerAsWell = false)
        {
            if ((Frames.Count - 1) > 0 && Frames.ElementAtOrDefault(frameIndex) != null)
            {
                var index = frameIndex;

                RemoveLayersFromVisualChildren();

                if (removeCaretakerAsWell)
                    _caretaker.RemoveCaretaker(Frames[index].Id);
                Frames.RemoveAt(index);

                if (Frames.Count == 1)
                {
                    ActiveFrameIndex = 0;
                }
                else
                {
                    ActiveFrameIndex = index - 1;
                }

                _caretaker.ActiveKey = Frames[ActiveFrameIndex].Id;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.FRAME_REMOVE, ActiveFrameIndex));

                SetActiveLayer();
            }
        }

        /// <summary>
        /// Sets a frame active (selected).
        /// </summary>
        /// <param name="frame"></param>
        public void SetActiveFrame(DrawingFrame frame)
        {
            var index = Frames.FindIndex(f => f.FrameName == frame.FrameName);
            if (index != -1)
            {
                var tmp = ActiveFrameIndex;
                ActiveFrameIndex = index;
                _caretaker.ActiveKey = Frames[ActiveFrameIndex].Id;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.FRAME_NEW_ACTIVE_INDEX, ActiveFrameIndex));

                SetActiveLayer();
            }
        }

        /// <summary>
        /// Resets a frame, leaving only one blank layer. Irreversible operation.
        /// </summary>
        /// <param name="frameIndex">Index of frame to reset.</param>
        public void ResetDrawingFrame(int frameIndex)
        {
            // Getting frame to clear.
            var tmp = Frames[frameIndex];

            // Update layer list on the visual tree and clear previous layers.
            RemoveLayersFromVisualChildren();
            Frames[frameIndex].Layers.Clear();
            Frames[frameIndex].Layers.Add(new DrawingLayer(this, DefaultLayerName));
            AddLayersToVisualChildren();

            // Clear undo-redo for layers in the frame.
            _caretaker.Clear(Frames[frameIndex].Id);

            // Clear undo-redo for frames.
            _caretaker.Clear(FramesCaretakerKey);

            FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs(IPixelEditorEventType.CLEAR, frameIndex));
            LayerListChanged?.Invoke(this, new PixelEditorLayerEventArgs(IPixelEditorEventType.CLEAR, 0));

            SetActiveLayer();
        }

        /// <summary>
        /// Undo previous operation applied on frames.
        /// </summary>
        public void UndoFrameOperation() => _caretaker.Undo(FramesCaretakerKey);

        /// <summary>
        /// Redo previous operation that was undone.
        /// </summary>
        public void RedoFrameOperation() => _caretaker.Redo(FramesCaretakerKey);
    }
}