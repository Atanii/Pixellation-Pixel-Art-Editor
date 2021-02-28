using System;

namespace Pixellation.Components.Event
{
    public class PixelEditorLayerEventArgs : EventArgs
    {
        public int EditorEventTypeValue { get; private set; }

        public int NewIndexOfActiveLayer { get; private set; } = -1;

        public static new PixelEditorLayerEventArgs Empty;

        public PixelEditorLayerEventArgs() : base()
        {
        }

        public PixelEditorLayerEventArgs(int editorEventTypeValue, int newIndexOfActiveLayer)
        {
            EditorEventTypeValue = editorEventTypeValue;
            NewIndexOfActiveLayer = newIndexOfActiveLayer;
        }

        public PixelEditorLayerEventArgs(int editorEventTypeValue)
        {
            EditorEventTypeValue = editorEventTypeValue;
        }
    }
}