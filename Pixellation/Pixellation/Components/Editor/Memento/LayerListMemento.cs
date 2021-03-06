using Pixellation.Components.Event;
using Pixellation.MementoPattern;

namespace Pixellation.Components.Editor.Memento
{
    /// <summary>
    /// Class representing a saved state of a layerlist.
    /// </summary>
    public class LayerListMemento : BaseMemento<LayerListMemento, IPixelEditorEventType>
    {
        /// <summary>
        /// Index of saved layer.
        /// </summary>
        public int LayerIndex { get; private set; }

        /// <summary>
        /// Width of layers in pixels.
        /// </summary>
        public int PixelWidth { get; private set; }

        /// <summary>
        /// Height of layers in pixels.
        /// </summary>
        public int PixelHeight { get; private set; }

        /// <summary>
        /// Inits a saved state for a layerlist.
        /// </summary>
        /// <param name="originatorHandler">Helper class for saving the state.</param>
        /// <param name="typeValue">Value of <see cref="IPixelEditorEventType"/>.</param>
        /// <param name="layerIndex"><see cref="LayerIndex"/>.</param>
        /// <param name="pixelWidth"><see cref="PixelWidth"/>.</param>
        /// <param name="pixelHeight"><see cref="PixelHeight"/>.</param>
        public LayerListMemento(IOriginatorHandler<LayerListMemento, IPixelEditorEventType> originatorHandler, int typeValue, int layerIndex, int pixelWidth, int pixelHeight)
            : base(typeValue, originatorHandler)
        {
            LayerIndex = layerIndex;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
        }

        /// <summary>
        /// Restore saved state.
        /// </summary>
        /// <returns>Memento for redo (or undo in case of redo).</returns>
        public override IMemento<IPixelEditorEventType> Restore()
        {
            return _originatorHandler.HandleRestore(this);
        }
    }
}