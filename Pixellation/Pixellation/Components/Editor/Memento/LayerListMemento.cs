using Pixellation.Utils.MementoPattern;

namespace Pixellation.Components.Editor.Memento
{
    public class LayerListMemento : BaseMemento<LayerListMemento, IEditorEventType>
    {
        public int LayerIndex { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }

        public LayerListMemento(IOriginatorHandler<LayerListMemento, IEditorEventType> originator, int typeValue, int layerIndex, int pixelWidth, int pixelHeight)
            : base(typeValue, originator)
        {
            LayerIndex = layerIndex;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
        }

        public override IMemento<IEditorEventType> Restore()
        {
            return _originatorHandler.HandleRestore(this);
        }
    }
}