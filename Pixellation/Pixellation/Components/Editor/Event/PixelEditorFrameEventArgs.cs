using System;

namespace Pixellation.Components.Event
{
    public class PixelEditorFrameEventArgs : EventArgs
    {
        public int EditorEventTypeValue { get; private set; }

        public int NewIndexOfActiveFrame { get; private set; }

        public static new PixelEditorLayerEventArgs Empty;

        public PixelEditorFrameEventArgs() : base()
        {
        }

        public PixelEditorFrameEventArgs(int editorEventTypeValue, int newIndexOfActiveFrame)
        {
            EditorEventTypeValue = editorEventTypeValue;
            NewIndexOfActiveFrame = newIndexOfActiveFrame;
        }
    }
}