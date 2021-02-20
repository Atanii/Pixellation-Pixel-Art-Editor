using Pixellation.Components.Editor.Memento;
using Pixellation.Utils.MementoPattern;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor : IOriginatorHandler<LayerMemento, IEditorEventType>, IOriginatorHandler<LayerListMemento, IEditorEventType>, IOriginator<LayerListMemento, IEditorEventType>
    {
        public LayerListMemento GetMemento(int type)
        {
            return new LayerListMemento(this, type, GetIndex(_activeLayer));
        }

        public void Restore(LayerListMemento mem)
        {
            switch (mem.GetMementoType())
            {
                case IEditorEventType.MOVELAYERUP:
                    MoveLayerDown(mem.LayerIndex);
                    break;

                case IEditorEventType.MOVELAYERDOWN:
                    MoveLayerUp(mem.LayerIndex);
                    break;

                default:
                    break;
            }
        }

        public void SaveState(int eTypeValue, int selectedLayerIndex)
        {
            switch (eTypeValue)
            {
                case IEditorEventType.MOVELAYERUP:
                case IEditorEventType.MOVELAYERDOWN:
                    _mementoCaretaker.Save(GetMemento(eTypeValue));
                    break;

                default:
                    _mementoCaretaker.Save(Layers[selectedLayerIndex].GetMemento(eTypeValue));
                    break;
            }
        }

        public IMemento<IEditorEventType> HandleRestore(LayerMemento mem)
        {
            LayerMemento redoMem;
            DrawingLayer originator;
            int origIndex;
            switch (mem.GetMementoType())
            {
                case IEditorEventType.LAYER_PIXELS_CHANGED:
                    originator = Layers.Find(x => x.LayerName == mem.LayerName);
                    if (originator != null)
                    {
                        redoMem = originator.GetMemento(IEditorEventType.LAYER_PIXELS_CHANGED);
                        originator.Restore(mem);
                        RefreshVisualsThenSignalUpdate();
                        return redoMem;
                    }
                    return null;

                case IEditorEventType.REMOVELAYER:
                    originator = new DrawingLayer(this, mem);
                    redoMem = originator.GetMemento(IEditorEventType.ADDLAYER);
                    AddLayer(originator, mem.LayerIndex);
                    return redoMem;

                case IEditorEventType.ADDLAYER:
                    origIndex = Layers.FindIndex(x => x.LayerName == mem.LayerName);
                    if (origIndex != -1)
                    {
                        redoMem = Layers[origIndex].GetMemento(IEditorEventType.REMOVELAYER);
                        RemoveLayer(origIndex);
                        return redoMem;
                    }
                    return null;

                default:
                    return null;
            }
        }

        public IMemento<IEditorEventType> HandleRestore(LayerListMemento mem)
        {
            LayerListMemento redoMem;
            switch (mem.GetMementoType())
            {
                case IEditorEventType.MOVELAYERUP:
                    redoMem = GetMemento(IEditorEventType.MOVELAYERDOWN);
                    Restore(mem);
                    return redoMem;

                case IEditorEventType.MOVELAYERDOWN:
                    redoMem = GetMemento(IEditorEventType.MOVELAYERUP);
                    Restore(mem);
                    return redoMem;

                default:
                    return null;
            }
        }
    }
}