using System;

namespace Pixellation.Components.Event
{
    /// <summary>
    /// Args for events regarding layers.
    /// </summary>
    public class PixelEditorLayerEventArgs : EventArgs
    {
        /// <summary>
        /// Value of corresponding <see cref="IPixelEditorEventType"/>.
        /// </summary>
        public int EditorEventTypeValue { get; private set; }

        /// <summary>
        /// New selected layerindex after the event. Default -1 means no change in selection.
        /// </summary>
        public int NewIndexOfActiveLayer { get; private set; } = -1;

        /// <summary>
        /// Empty event.
        /// </summary>
        public static new PixelEditorLayerEventArgs Empty;

        /// <summary>
        /// Inits an event with default property values.
        /// </summary>
        public PixelEditorLayerEventArgs() : base()
        {
        }

        /// <summary>
        /// Inits an event.
        /// </summary>
        /// <param name="editorEventTypeValue"><see cref="EditorEventTypeValue"/>.</param>
        /// <param name="newIndexOfActiveLayer"><see cref="NewIndexOfActiveLayer"/>.</param>
        public PixelEditorLayerEventArgs(int editorEventTypeValue, int newIndexOfActiveLayer)
        {
            EditorEventTypeValue = editorEventTypeValue;
            NewIndexOfActiveLayer = newIndexOfActiveLayer;
        }

        /// <summary>
        /// Inits an event.
        /// </summary>
        /// <param name="editorEventTypeValue"><see cref="EditorEventTypeValue"/>.</param>
        public PixelEditorLayerEventArgs(int editorEventTypeValue)
        {
            EditorEventTypeValue = editorEventTypeValue;
        }
    }
}