using Pixellation.Utils.MementoPattern;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Editor.Memento
{
    public class LayerMemento : BaseMemento<LayerMemento, IEditorEventType>
    {
        public int LayerIndex { get; private set; }
        public string LayerName { get; private set; }
        public double Opacity { get; private set; }
        public bool Visible { get; private set; }
        public WriteableBitmap Bitmap { get; private set; }

        public LayerMemento(IOriginatorHandler<LayerMemento, IEditorEventType> originator, int mTypevalue, int layerIndex, DrawingLayer original)
            : base(mTypevalue, originator)
        {
            LayerIndex = layerIndex;

            LayerName = original.LayerName;
            Opacity = original.Opacity;
            Visible = original.Visible;
            Bitmap = original.GetWriteableBitmap();
        }

        public override IMemento<IEditorEventType> Restore()
        {
            return _originatorHandler.HandleRestore(this);
        }
    }
}