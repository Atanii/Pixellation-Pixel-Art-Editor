using System;

namespace Pixellation.Components.Editor
{
    public class PixelEditorEventArgs : EventArgs
    {
        public int EditorEventTypeValue { get; private set; }
        public int[] IndexesOfModifiedLayers { get; private set; }
        public int OldIndexOfActiveLayer { get; private set; }
        public int NewIndexOfActiveLayer { get; private set; }

        public static new PixelEditorEventArgs Empty;

        public PixelEditorEventArgs() : base() {}

        public PixelEditorEventArgs(int editorEventTypeValue, int oldIndexOfActiveLayer, int newIndexOfActiveLayer, int[] indexesOfModifiedLayers)
        {
            EditorEventTypeValue = editorEventTypeValue;
            OldIndexOfActiveLayer = oldIndexOfActiveLayer;
            NewIndexOfActiveLayer = newIndexOfActiveLayer;
            IndexesOfModifiedLayers = indexesOfModifiedLayers;
        }
    }
}