using Pixellation.MementoPattern;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor.Memento
{
    public class LayerMemento : BaseMemento<LayerMemento, IPixelEditorEventType>
    {
        public int LayerIndex { get; private set; }
        public string LayerName { get; private set; }
        public double Opacity { get; private set; }
        public bool Visible { get; private set; }
        public WriteableBitmap Bitmap { get; private set; }
        public LayerMemento ChainedOperationMemento { get; private set; }

        public LayerMemento(IOriginatorHandler<LayerMemento, IPixelEditorEventType> originator, int mTypevalue, int layerIndex, DrawingLayer original)
            : base(mTypevalue, originator)
        {
            LayerIndex = layerIndex;

            LayerName = original.LayerName;
            Opacity = original.Opacity;
            Visible = original.Visible;
            Bitmap = original.GetWriteableBitmap();
        }

        public void SetChainedMemento(LayerMemento chained)
        {
            if (chained != null)
            {
                ChainedOperationMemento = chained;
            }
        }

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