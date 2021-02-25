using System;

namespace Pixellation.Components.Editor
{
    public class PixelEditorLayerEventArgs : EventArgs
    {
        public int EditorEventTypeValue { get; private set; }
        public int[] IndexesOfModifiedLayers { get; private set; } = new int[] { };
        public int OldIndexOfActiveLayer { get; private set; } = -1;
        public int NewIndexOfActiveLayer { get; private set; } = -1;

        public static new PixelEditorLayerEventArgs Empty;

        public PixelEditorLayerEventArgs() : base() {}

        public PixelEditorLayerEventArgs(int editorEventTypeValue, int oldIndexOfActiveLayer, int newIndexOfActiveLayer, int[] indexesOfModifiedLayers)
        {
            EditorEventTypeValue = editorEventTypeValue;
            OldIndexOfActiveLayer = oldIndexOfActiveLayer;
            NewIndexOfActiveLayer = newIndexOfActiveLayer;
            IndexesOfModifiedLayers = indexesOfModifiedLayers;
        }

        public PixelEditorLayerEventArgs(int editorEventTypeValue)
        {
            EditorEventTypeValue = editorEventTypeValue;
        }
    }
}