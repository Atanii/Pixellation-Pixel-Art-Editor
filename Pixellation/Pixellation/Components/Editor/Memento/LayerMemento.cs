using Pixellation.Components.Event;
using Pixellation.MementoPattern;
using System;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor.Memento
{
    /// <summary>
    /// Class representing a saved state of a layer (or two layers in case of a one-step composite operation).
    /// </summary>
    public class LayerMemento : BaseMemento<LayerMemento, IPixelEditorEventType>
    {
        /// <summary>
        /// Index of saved layer.
        /// </summary>
        public int LayerIndex { get; private set; }

        /// <summary>
        /// Name of saved layer.
        /// </summary>
        public string LayerName { get; private set; }

        /// <summary>
        /// Saved state of layer opacity.
        /// </summary>
        public double Opacity { get; private set; }

        /// <summary>
        /// Saves state of layer visibility.
        /// </summary>
        public bool Visible { get; private set; }

        /// <summary>
        /// Bitmap of layer.
        /// </summary>
        public WriteableBitmap Bitmap { get; private set; }

        /// <summary>
        /// Second layer memento in case of composit operations like merging two layers.
        /// </summary>
        public LayerMemento ChainedOperationMemento { get; private set; }

        /// <summary>
        /// Unique id of layer.
        /// </summary>
        public Guid LayerGuid { get; private set; }

        /// <summary>
        /// Inits a saved state for a layer.
        /// </summary>
        /// <param name="originatorHandler">Helper class for saving the state.</param>
        /// <param name="mTypevalue">Value of <see cref="IPixelEditorEventType"/>.</param>
        /// <param name="layerIndex"><see cref="LayerIndex"/></param>
        /// <param name="original">Layer to save.</param>
        public LayerMemento(IOriginatorHandler<LayerMemento, IPixelEditorEventType> originatorHandler, int mTypevalue, int layerIndex, DrawingLayer original)
            : base(mTypevalue, originatorHandler)
        {
            LayerIndex = layerIndex;

            LayerName = original.LayerName;
            Opacity = original.Opacity;
            Visible = original.Visible;
            Bitmap = original.GetWriteableBitmap();
            LayerGuid = original.LayerGuid;
        }

        /// <summary>
        /// Sets a secondary memento in case of composite operations (operations consisting of multiple sub-operations).
        /// </summary>
        /// <param name="chained">Secondary memento.</param>
        public void SetChainedMemento(LayerMemento chained)
        {
            if (chained != null)
            {
                ChainedOperationMemento = chained;
            }
        }

        /// <summary>
        /// Restore saved state.
        /// </summary>
        /// <returns>Memento for redo (or undo in case of redo).</returns>
        public override IMemento<IPixelEditorEventType> Restore()
        {
            if (ChainedOperationMemento != null)
            {
                _originatorHandler.HandleRestore(ChainedOperationMemento);
            }
            return _originatorHandler.HandleRestore(this);
        }
    }
}