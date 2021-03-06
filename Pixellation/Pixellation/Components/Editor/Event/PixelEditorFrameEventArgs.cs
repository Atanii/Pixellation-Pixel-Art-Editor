using System;

namespace Pixellation.Components.Event
{
    /// <summary>
    /// Args for events regarding frames.
    /// </summary>
    public class PixelEditorFrameEventArgs : EventArgs
    {
        /// <summary>
        /// Value of corresponding <see cref="IPixelEditorEventType"/>.
        /// </summary>
        public int EditorEventTypeValue { get; private set; }

        /// <summary>
        /// New selected frameindex after the event.
        /// </summary>
        public int NewIndexOfActiveFrame { get; private set; }

        /// <summary>
        /// Empty event.
        /// </summary>
        public static new PixelEditorLayerEventArgs Empty;

        /// <summary>
        /// Inits an event with default property values.
        /// </summary>
        public PixelEditorFrameEventArgs() : base()
        {
        }

        /// <summary>
        /// Inits an event.
        /// </summary>
        /// <param name="editorEventTypeValue"><see cref="EditorEventTypeValue"/>.</param>
        /// <param name="newIndexOfActiveFrame"><see cref="NewIndexOfActiveFrame"/>.</param>
        public PixelEditorFrameEventArgs(int editorEventTypeValue, int newIndexOfActiveFrame)
        {
            EditorEventTypeValue = editorEventTypeValue;
            NewIndexOfActiveFrame = newIndexOfActiveFrame;
        }
    }
}