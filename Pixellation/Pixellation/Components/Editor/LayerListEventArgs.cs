using System;

namespace Pixellation.Components.Editor
{
    public class LayerListEventArgs : EventArgs
    {
        public int EditorEventTypeValue { get; private set; } = IEditorEventType.NONE;
        public int[] IndexesOfModifiedLayers { get; private set; }
        public int OldIndexOfActiveLayer { get; private set; }
        public int NewIndexOfActiveLayer { get; private set; }

        public static new LayerListEventArgs Empty;

        public LayerListEventArgs() : base() {}

        public LayerListEventArgs(int editorEventTypeValue, int oldIndexOfActiveLayer, int newIndexOfActiveLayer, int[] indexesOfModifiedLayers)
        {
            EditorEventTypeValue = editorEventTypeValue;
            OldIndexOfActiveLayer = oldIndexOfActiveLayer;
            NewIndexOfActiveLayer = newIndexOfActiveLayer;
            IndexesOfModifiedLayers = indexesOfModifiedLayers;
        }
    }
}