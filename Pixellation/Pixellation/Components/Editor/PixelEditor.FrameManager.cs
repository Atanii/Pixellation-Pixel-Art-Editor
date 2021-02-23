using Pixellation.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor : IFrameManager
    {
        public void AddDrawingFrame(int frameIndex, string name)
        {
            if (Frames.Count == 0 && frameIndex == 0)
            {
                var tmp = new DrawingFrame("Default", this);
                _caretaker.InitCaretaker(tmp.Id);
                Frames.Insert(frameIndex, tmp);

                ActiveFrameIndex = 0;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
                (
                    IPixelEditorEventType.FRAME_ADD,
                    frameIndex, frameIndex, new int[] { frameIndex }
                ));

                SetActiveLayer();
            }
            else if (Frames.Count >= (frameIndex + 1))
            {
                var tmp = new DrawingFrame(name, this, true, true);
                _caretaker.InitCaretaker(tmp.Id);
                Frames.Insert(frameIndex + 1, tmp);

                ActiveFrameIndex = frameIndex + 1;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
                (
                    IPixelEditorEventType.FRAME_ADD,
                    frameIndex, frameIndex + 1, new int[] { frameIndex }
                ));

                SetActiveLayer();
            }
        }

        public void DuplicateDrawingFrame(int frameIndex)
        {
            if (Frames.Count >= (frameIndex + 1))
            {
                var tmp = Frames[frameIndex].Clone();
                _caretaker.InitCaretaker(tmp.Id);
                Frames.Insert(frameIndex + 1, tmp);

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
                (
                    IPixelEditorEventType.FRAME_DUPLICATE,
                    frameIndex, frameIndex, new int[] { frameIndex }
                ));
            }
        }

        public DrawingFrame GetActiveDrawingFrame() => _activeFrame;

        public int GetActiveFrameIndex() => ActiveFrameIndex;

        public void MergeDrawingFrameIntoLeftNeighbour(int frameIndex)
        {
            if (frameIndex > 0 && Frames.Count >= (frameIndex + 1))
            {
                var tmp = Frames[frameIndex];
                Frames[frameIndex - 1].Layers.AddRange(tmp.Layers);
                Frames.RemoveAt(frameIndex);

                ActiveFrameIndex = frameIndex - 1;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
                (
                    IPixelEditorEventType.FRAME_MERGE_TO_LEFT,
                    frameIndex, frameIndex - 1, new int[] { frameIndex, frameIndex - 1 }
                ));

                SetActiveLayer();
            }
        }

        public void MoveDrawingFrameLeft(int frameIndex)
        {
            if (frameIndex > 0 && Frames.Count >= (frameIndex + 1))
            {
                var tmp = Frames[frameIndex];
                Frames.RemoveAt(frameIndex);
                Frames.Insert(frameIndex - 1, tmp);

                ActiveFrameIndex = frameIndex - 1;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
                (
                    IPixelEditorEventType.FRAME_MOVE_LEFT,
                    frameIndex, frameIndex - 1, new int[] { frameIndex, frameIndex - 1 }
                ));

                SetActiveLayer();
            }
        }

        public void MoveDrawingFrameRight(int frameIndex)
        {
            if (Frames.Count > (frameIndex + 1))
            {
                var tmp = Frames[frameIndex];
                Frames.RemoveAt(frameIndex);
                Frames.Insert(frameIndex + 1, tmp);

                ActiveFrameIndex = frameIndex + 1;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
                (
                    IPixelEditorEventType.FRAME_MOVE_RIGHT,
                    frameIndex, frameIndex + 1, new int[] { frameIndex, frameIndex + 1 }
                ));

                SetActiveLayer();
            }
        }

        public void RemoveDrawingFrame(int frameIndex)
        {
            if ((Frames.Count - 1) > 0 && Frames.ElementAtOrDefault(frameIndex) != null)
            {
                var index = frameIndex;

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

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
                (
                    IPixelEditorEventType.FRAME_REMOVE,
                    index, ActiveFrameIndex, new int[] { index }
                ));

                SetActiveLayer();
            }
        }

        public List<DrawingFrame> GetFrames() => Frames;

        public void SetActiveFrame(DrawingFrame frame)
        {
            Debug.WriteLine($"Setting frame ({frame.FrameName}) as active frame.");

            var index = Frames.FindIndex(f => f.FrameName == frame.FrameName);
            if (index != -1)
            {
                var tmp = ActiveFrameIndex;
                ActiveFrameIndex = index;
                _caretaker.ActiveKey = Frames[ActiveFrameIndex].Id;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
                (
                    IPixelEditorEventType.FRAME_NEW_ACTIVE_INDEX,
                    tmp, ActiveFrameIndex, new int[] { }
                ));

                SetActiveLayer();
            }
        }
    }
}