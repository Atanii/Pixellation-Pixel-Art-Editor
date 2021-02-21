using Pixellation.Components.Editor.Memento;
using Pixellation.Utils.MementoPattern;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor : IOriginatorHandler<LayerMemento, IEditorEventType>, IOriginatorHandler<LayerListMemento, IEditorEventType>, IOriginator<LayerListMemento, IEditorEventType>
    {
        public LayerListMemento GetMemento(int type)
        {
            return new LayerListMemento(this, type, GetIndex(_activeLayer), PixelWidth, PixelHeight);
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

                case IEditorEventType.MIRROR_HORIZONTAL:
                    Mirror(true, false);
                    break;
                case IEditorEventType.MIRROR_HORIZONTAL_ALL:
                    Mirror(true, true);
                    break;
                case IEditorEventType.MIRROR_VERTICAL:
                    Mirror(false, false);
                    break;
                case IEditorEventType.MIRROR_VERTICAL_ALL:
                    Mirror(false, true);
                    break;

                case IEditorEventType.ROTATE:
                    Rotate(false, true);
                    break;
                case IEditorEventType.ROTATE_ALL:
                    Rotate(true, true);
                    break;
                case IEditorEventType.ROTATE_COUNTERCLOCKWISE:
                    Rotate(false, false);
                    break;
                case IEditorEventType.ROTATE_COUNTERCLOCKWISE_ALL:
                    Rotate(true, false);
                    break;

                case IEditorEventType.RESIZE:
                    Resize(mem.PixelWidth, mem.PixelHeight);
                    break;

                default:
                    break;
            }
        }

        public void SaveState(int eTypeValue, int selectedLayerIndex)
        {
            switch (eTypeValue)
            {
                // LayerListMemento
                case IEditorEventType.MOVELAYERUP:
                case IEditorEventType.MOVELAYERDOWN:
                case IEditorEventType.MIRROR_HORIZONTAL:
                case IEditorEventType.MIRROR_HORIZONTAL_ALL:
                case IEditorEventType.MIRROR_VERTICAL:
                case IEditorEventType.MIRROR_VERTICAL_ALL:
                case IEditorEventType.ROTATE:
                case IEditorEventType.ROTATE_ALL:
                case IEditorEventType.ROTATE_COUNTERCLOCKWISE:
                case IEditorEventType.ROTATE_COUNTERCLOCKWISE_ALL:
                case IEditorEventType.RESIZE:
                    _mementoCaretaker.Save(GetMemento(eTypeValue));
                    break;

                // LayerMemento
                case IEditorEventType.MERGELAYER:
                    var mergeMemento = Layers[selectedLayerIndex].GetMemento(eTypeValue);
                    mergeMemento.SetChainedMemento(Layers[selectedLayerIndex + 1].GetMemento(IEditorEventType.REMOVELAYER));
                    _mementoCaretaker.Save(mergeMemento);
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

            var typeValue = mem.GetMementoType();
            switch (typeValue)
            {
                case IEditorEventType.LAYER_PIXELS_CHANGED:
                case IEditorEventType.LAYER_OPACITYCHANGED:
                case IEditorEventType.LAYER_VISIBILITYCHANGED:
                    origIndex = Layers.FindIndex(x => x.LayerName == mem.LayerName);
                    if (origIndex != -1)
                    {
                        redoMem = Layers[origIndex].GetMemento(typeValue);
                        Layers[origIndex].Restore(mem);
                        RefreshVisualsThenSignalUpdate();
                        return redoMem;
                    }
                    return null;

                case IEditorEventType.MERGELAYER:
                    origIndex = Layers.FindIndex(x => x.LayerName == mem.LayerName);
                    if (origIndex != -1)
                    {
                        redoMem = Layers[origIndex].GetMemento(-typeValue);
                        Layers[origIndex].Restore(mem);

                        // Set original pre-merge selected layerindex
                        LayerListChanged?.Invoke(this, new LayerListEventArgs
                        (
                            IEditorEventType.REVERSE_MERGELAYER,
                            origIndex, origIndex,
                            new int[] { origIndex, origIndex + 1 }
                        ));

                        RefreshVisualsThenSignalUpdate();
                        return redoMem;
                    }
                    return null;

                case IEditorEventType.REMOVELAYER:
                    originator = new DrawingLayer(this, mem);
                    redoMem = originator.GetMemento(-typeValue);
                    AddLayer(originator, mem.LayerIndex);
                    return redoMem;

                case IEditorEventType.ADDLAYER:
                case IEditorEventType.DUPLICATELAYER:
                    origIndex = Layers.FindIndex(x => x.LayerName == mem.LayerName);
                    if (origIndex != -1)
                    {
                        redoMem = Layers[origIndex].GetMemento(IEditorEventType.REMOVELAYER);
                        RemoveLayer(origIndex);
                        return redoMem;
                    }
                    return null;

                case IEditorEventType.RENAMELAYER:
                    originator = Layers[mem.LayerIndex];
                    if (originator != null)
                    {
                        redoMem = originator.GetMemento(typeValue);
                        originator.Restore(mem);
                        RefreshVisualsThenSignalUpdate();
                        return redoMem;
                    }
                    return null;


                case IEditorEventType.REVERSE_MERGELAYER:
                    origIndex = Layers.FindIndex(x => x.LayerName == mem.LayerName);
                    if (origIndex != -1 && Layers.Count >= (origIndex + 2))
                    {
                        redoMem = Layers[origIndex].GetMemento(-typeValue);
                        redoMem.SetChainedMemento(Layers[origIndex + 1].GetMemento(IEditorEventType.REMOVELAYER));
                        MergeLayerDownward(origIndex);
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
            var typeValue = mem.GetMementoType();
            switch (typeValue)
            {
                case IEditorEventType.MOVELAYERUP:
                case IEditorEventType.MOVELAYERDOWN:
                case IEditorEventType.ROTATE:
                case IEditorEventType.ROTATE_ALL:
                case IEditorEventType.ROTATE_COUNTERCLOCKWISE:
                case IEditorEventType.ROTATE_COUNTERCLOCKWISE_ALL:
                    redoMem = GetMemento(-typeValue);
                    Restore(mem);
                    return redoMem;

                case IEditorEventType.MIRROR_HORIZONTAL:
                case IEditorEventType.MIRROR_HORIZONTAL_ALL:
                case IEditorEventType.MIRROR_VERTICAL:
                case IEditorEventType.MIRROR_VERTICAL_ALL:
                case IEditorEventType.RESIZE:
                    redoMem = GetMemento(typeValue);
                    Restore(mem);
                    return redoMem;

                default:
                    return null;
            }
        }
    }
}