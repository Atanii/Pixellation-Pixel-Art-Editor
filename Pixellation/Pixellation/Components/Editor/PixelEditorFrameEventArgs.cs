using System;

namespace Pixellation.Components.Editor
{
    public class PixelEditorFrameEventArgs : EventArgs
    {
        public int EditorEventTypeValue { get; private set; }
        public int[] IndexesOfModifiedFrames { get; private set; }
        public int OldIndexOfActiveFrame { get; private set; }
        public int NewIndexOfActiveFrame { get; private set; }

        public static new PixelEditorLayerEventArgs Empty;

        public PixelEditorFrameEventArgs() : base() {}

        public PixelEditorFrameEventArgs(int editorEventTypeValue, int oldIndexOfActiveFrame, int newIndexOfActiveFrame, int[] indexesOfModifiedFrames)
        {
            EditorEventTypeValue = editorEventTypeValue;
            OldIndexOfActiveFrame = oldIndexOfActiveFrame;
            NewIndexOfActiveFrame = newIndexOfActiveFrame;
            IndexesOfModifiedFrames = indexesOfModifiedFrames;
        }
    }
}