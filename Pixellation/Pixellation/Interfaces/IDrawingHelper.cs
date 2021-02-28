using Pixellation.Components.Editor;
using Pixellation.Components.Editor.Memento;
using Pixellation.Components.Event;
using Pixellation.MementoPattern;

namespace Pixellation.Interfaces
{
    /// <summary>
    /// Interface for a class helping the functionality of <see cref="DrawingLayer"/> and <see cref="DrawingFrame"/> objects.
    /// </summary>
    public interface IDrawingHelper :
        IOriginatorHandler<LayerMemento, IPixelEditorEventType>,
        IOriginatorHandler<LayerListMemento, IPixelEditorEventType>,
        IOriginatorHandler<FrameMemento, IPixelEditorEventType>
    {
        /// <summary>
        /// Current magnification of the edited image.
        /// </summary>
        int Magnification { get; }

        /// <summary>
        /// Width of edited image in pixels.
        /// </summary>
        int PixelWidth { get; }

        /// <summary>
        /// Height of edited image in pixels.
        /// </summary>
        int PixelHeight { get; }

        /// <summary>
        /// Indicates if TiledMode is turned on or not.
        /// </summary>
        bool TiledModeOn { get; }

        /// <summary>
        /// Opacity for tiles in tiled mode.
        /// </summary>
        float TiledOpacity { get; }

        /// <summary>
        /// Unique id of the selected frame.
        /// </summary>
        string ActiveFrameId { get; }

        /// <summary>
        /// Index of the selected frame.
        /// </summary>
        public int ActiveFrameIndex { get; }

        /// <summary>
        /// Sets a frame active (selected).
        /// </summary>
        /// <param name="frame"></param>
        public void SetActiveFrame(DrawingFrame frame);

        /// <summary>
        /// Gets index for a layer.
        /// </summary>
        /// <param name="layer">Layer the index is asked for.</param>
        /// <returns>Index of the given layer.</returns>
        public int GetIndex(DrawingLayer layer);

        /// <summary>
        /// Refreshes editor visuals.
        /// </summary>
        public void RefreshVisualsThenSignalUpdate();

        /// <summary>
        /// Saves a layerstate for undo.
        /// </summary>
        /// <param name="eTypeValue">Value of the given <see cref="IPixelEditorEventType"/>.</param>
        /// <param name="selectedLayerIndex">Index of layer to save state for.</param>
        public void SaveState(int eTypeValue, int selectedLayerIndex);

        /// <summary>
        /// Index of currently selected layer.
        /// </summary>
        public int ActiveLayerIndex { get; }

        /// <summary>
        /// Editable width in pixels with applied magnification.
        /// </summary>
        public int MagnifiedWidth { get; }

        /// <summary>
        /// Editable height in pixels with applied magnification.
        /// </summary>
        public int MagnifiedHeight { get; }
    }
}