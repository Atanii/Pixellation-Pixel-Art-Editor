using Pixellation.Components.Editor.Memento;
using Pixellation.Utils.MementoPattern;

namespace Pixellation.Components.Editor
{
    public partial class PixelEditor : IOriginatorHandler<LayerMemento, IPixelEditorEventType>, IOriginatorHandler<LayerListMemento, IPixelEditorEventType>, IOriginator<LayerListMemento, IPixelEditorEventType>
    {
        public LayerListMemento GetMemento(int type)
        {
            return new LayerListMemento(this, type, GetIndex(_activeLayer), PixelWidth, PixelHeight);
        }

        public void Restore(LayerListMemento mem)
        {
            switch (mem.GetMementoType())
            {
                case IPixelEditorEventType.MOVELAYERUP:
                    MoveLayerDown(mem.LayerIndex);
                    break;
                case IPixelEditorEventType.MOVELAYERDOWN:
                    MoveLayerUp(mem.LayerIndex);
                    break;

                case IPixelEditorEventType.MIRROR_HORIZONTAL:
                    Mirror(true, false);
                    break;
                case IPixelEditorEventType.MIRROR_HORIZONTAL_ALL:
                    Mirror(true, true);
                    break;
                case IPixelEditorEventType.MIRROR_VERTICAL:
                    Mirror(false, false);
                    break;
                case IPixelEditorEventType.MIRROR_VERTICAL_ALL:
                    Mirror(false, true);
                    break;

                case IPixelEditorEventType.ROTATE:
                    Rotate(false, true);
                    break;
                case IPixelEditorEventType.ROTATE_ALL:
                    Rotate(true, true);
                    break;
                case IPixelEditorEventType.ROTATE_COUNTERCLOCKWISE:
                    Rotate(false, false);
                    break;
                case IPixelEditorEventType.ROTATE_COUNTERCLOCKWISE_ALL:
                    Rotate(true, false);
                    break;

                case IPixelEditorEventType.RESIZE:
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
                case IPixelEditorEventType.MOVELAYERUP:
                case IPixelEditorEventType.MOVELAYERDOWN:
                case IPixelEditorEventType.MIRROR_HORIZONTAL:
                case IPixelEditorEventType.MIRROR_HORIZONTAL_ALL:
                case IPixelEditorEventType.MIRROR_VERTICAL:
                case IPixelEditorEventType.MIRROR_VERTICAL_ALL:
                case IPixelEditorEventType.ROTATE:
                case IPixelEditorEventType.ROTATE_ALL:
                case IPixelEditorEventType.ROTATE_COUNTERCLOCKWISE:
                case IPixelEditorEventType.ROTATE_COUNTERCLOCKWISE_ALL:
                case IPixelEditorEventType.RESIZE:
                    _mementoCaretaker.Save(GetMemento(eTypeValue));
                    break;

                // LayerMemento
                case IPixelEditorEventType.MERGELAYER:
                    var mergeMemento = Layers[selectedLayerIndex].GetMemento(eTypeValue);
                    mergeMemento.SetChainedMemento(Layers[selectedLayerIndex + 1].GetMemento(IPixelEditorEventType.REMOVELAYER));
                    _mementoCaretaker.Save(mergeMemento);
                    break;

                default:
                    _mementoCaretaker.Save(Layers[selectedLayerIndex].GetMemento(eTypeValue));
                    break;
            }
        }

        public IMemento<IPixelEditorEventType> HandleRestore(LayerMemento mem)
        {
            LayerMemento redoMem;
            DrawingLayer originator;
            int origIndex;

            var typeValue = mem.GetMementoType();
            switch (typeValue)
            {
                case IPixelEditorEventType.LAYER_PIXELS_CHANGED:
                case IPixelEditorEventType.LAYER_OPACITYCHANGED:
                case IPixelEditorEventType.LAYER_VISIBILITYCHANGED:
                    origIndex = Layers.FindIndex(x => x.LayerName == mem.LayerName);
                    if (origIndex != -1)
                    {
                        redoMem = Layers[origIndex].GetMemento(typeValue);
                        Layers[origIndex].Restore(mem);
                        RefreshVisualsThenSignalUpdate();
                        return redoMem;
                    }
                    return null;

                case IPixelEditorEventType.MERGELAYER:
                    origIndex = Layers.FindIndex(x => x.LayerName == mem.LayerName);
                    if (origIndex != -1)
                    {
                        redoMem = Layers[origIndex].GetMemento(-typeValue);
                        Layers[origIndex].Restore(mem);

                        // Set original pre-merge selected layerindex
                        LayerListChanged?.Invoke(this, new PixelEditorEventArgs
                        (
                            IPixelEditorEventType.REVERSE_MERGELAYER,
                            origIndex, origIndex,
                            new int[] { origIndex, origIndex + 1 }
                        ));

                        RefreshVisualsThenSignalUpdate();
                        return redoMem;
                    }
                    return null;

                case IPixelEditorEventType.REMOVELAYER:
                    originator = new DrawingLayer(this, mem);
                    redoMem = originator.GetMemento(-typeValue);
                    AddLayer(originator, mem.LayerIndex);
                    return redoMem;

                case IPixelEditorEventType.ADDLAYER:
                case IPixelEditorEventType.DUPLICATELAYER:
                    origIndex = Layers.FindIndex(x => x.LayerName == mem.LayerName);
                    if (origIndex != -1)
                    {
                        redoMem = Layers[origIndex].GetMemento(IPixelEditorEventType.REMOVELAYER);
                        RemoveLayer(origIndex);
                        return redoMem;
                    }
                    return null;

                case IPixelEditorEventType.RENAMELAYER:
                    originator = Layers[mem.LayerIndex];
                    if (originator != null)
                    {
                        redoMem = originator.GetMemento(typeValue);
                        originator.Restore(mem);
                        RefreshVisualsThenSignalUpdate();
                        return redoMem;
                    }
                    return null;


                case IPixelEditorEventType.REVERSE_MERGELAYER:
                    origIndex = Layers.FindIndex(x => x.LayerName == mem.LayerName);
                    if (origIndex != -1 && Layers.Count >= (origIndex + 2))
                    {
                        redoMem = Layers[origIndex].GetMemento(-typeValue);
                        redoMem.SetChainedMemento(Layers[origIndex + 1].GetMemento(IPixelEditorEventType.REMOVELAYER));
                        MergeLayerDownward(origIndex);
                        return redoMem;
                    }
                    return null;

                default:
                    return null;
            }
        }

        public IMemento<IPixelEditorEventType> HandleRestore(LayerListMemento mem)
        {
            LayerListMemento redoMem;
            var typeValue = mem.GetMementoType();
            switch (typeValue)
            {
                case IPixelEditorEventType.MOVELAYERUP:
                case IPixelEditorEventType.MOVELAYERDOWN:
                case IPixelEditorEventType.ROTATE:
                case IPixelEditorEventType.ROTATE_ALL:
                case IPixelEditorEventType.ROTATE_COUNTERCLOCKWISE:
                case IPixelEditorEventType.ROTATE_COUNTERCLOCKWISE_ALL:
                    redoMem = GetMemento(-typeValue);
                    Restore(mem);
                    return redoMem;

                case IPixelEditorEventType.MIRROR_HORIZONTAL:
                case IPixelEditorEventType.MIRROR_HORIZONTAL_ALL:
                case IPixelEditorEventType.MIRROR_VERTICAL:
                case IPixelEditorEventType.MIRROR_VERTICAL_ALL:
                case IPixelEditorEventType.RESIZE:
                    redoMem = GetMemento(typeValue);
                    Restore(mem);
                    return redoMem;

                default:
                    return null;
            }
        }
    }
}