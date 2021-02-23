using Pixellation.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor : IFrameManager
    {
        public void AddDrawingFrame(int frameIndex, string name)
        {
            if (Frames.Count >= (frameIndex + 1))
            {
                Frames.Insert(frameIndex + 1, new DrawingFrame(name, this));

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
                (
                    IPixelEditorEventType.FRAME_ADD,
                    frameIndex, frameIndex, new int[] { frameIndex }
                ));
            }
        }

        public void DuplicateDrawingFrame(int frameIndex)
        {
            if (Frames.Count >= (frameIndex + 1))
            {
                Frames.Insert(frameIndex + 1, Frames[frameIndex].Clone());

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
            }
        }

        public void RemoveDrawingFrame(int frameIndex)
        {
            if (frameIndex > 0 && Frames.Count >= (frameIndex + 1))
            {
                Frames.RemoveAt(frameIndex);

                ActiveFrameIndex = frameIndex - 1;

                FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
                (
                    IPixelEditorEventType.FRAME_REMOVE,
                    frameIndex, frameIndex - 1, new int[] { frameIndex }
                ));
            }
        }

        public List<DrawingFrame> GetFrames() => Frames;

        public void SetActiveFrame(DrawingFrame frame)
        {
            Debug.WriteLine($"Setting frame ({frame.FrameName}) as active frame.");

            var tmp = ActiveFrameIndex;
            ActiveFrameIndex = Frames.FindIndex(f => f.FrameName == frame.FrameName);

            FrameListChanged?.Invoke(this, new PixelEditorFrameEventArgs
            (
                IPixelEditorEventType.FRAME_NEW_ACTIVE_INDEX,
                tmp, ActiveFrameIndex, new int[] {}
            ));
        }
    }
}