using Pixellation.Utils.MementoPattern;

namespace Pixellation.Components.Editor.Memento
{
    public class LayerListMemento : BaseMemento<LayerListMemento, IEditorEventType>
    {
        public int LayerIndex { get; private set; }

        public LayerListMemento(IOriginatorHandler<LayerListMemento, IEditorEventType> originator, int typeValue, int layerIndex)
            : base(typeValue, originator)
        {
            LayerIndex = layerIndex;
        }

        public override IMemento<IEditorEventType> Restore()
        {
            return _originatorHandler.HandleRestore(this);
        }
    }
}