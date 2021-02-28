using Pixellation.Components.Event;
using Pixellation.MementoPattern;

namespace Pixellation.Components.Editor.Memento
{
    public class LayerListMemento : BaseMemento<LayerListMemento, IPixelEditorEventType>
    {
        public int LayerIndex { get; private set; }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }

        public LayerListMemento(IOriginatorHandler<LayerListMemento, IPixelEditorEventType> originator, int typeValue, int layerIndex, int pixelWidth, int pixelHeight)
            : base(typeValue, originator)
        {
            LayerIndex = layerIndex;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
        }

        public override IMemento<IPixelEditorEventType> Restore()
        {
            return _originatorHandler.HandleRestore(this);
        }
    }
}