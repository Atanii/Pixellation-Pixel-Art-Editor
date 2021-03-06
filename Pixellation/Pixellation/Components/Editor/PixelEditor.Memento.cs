﻿using Pixellation.Components.Editor.Memento;
using Pixellation.Components.Event;
using Pixellation.Interfaces;
using Pixellation.MementoPattern;

namespace Pixellation.Components.Editor
{
    /// <summary>
    /// Gives undo-redo support for frames, layers and for the list layers.
    /// </summary>
    public partial class PixelEditor : IDrawingHelper, IOriginator<LayerListMemento, IPixelEditorEventType>
    {
        /// <summary>
        /// Index that will be used for the next <see cref="LayerListMemento"/>.
        /// </summary>
        private int indexForLayerListMemento = -1;

        /// <summary>
        /// Gets a memento representing the current state of the layerlist.
        /// </summary>
        /// <param name="type">Value of <see cref="IPixelEditorEventType"/>.</param>
        /// <returns>Resulting memento.</returns>
        public LayerListMemento GetMemento(int type)
        {
            return new LayerListMemento(this, type, indexForLayerListMemento, PixelWidth, PixelHeight);
        }

        /// <summary>
        /// Restore layer-list state from a memento.
        /// </summary>
        /// <returns>Memento for redoing the undone operation.</returns>
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
                    Rotate(true);
                    break;

                case IPixelEditorEventType.ROTATE_COUNTERCLOCKWISE:
                    Rotate(false);
                    break;

                case IPixelEditorEventType.RESIZE:
                    Resize(mem.PixelWidth, mem.PixelHeight);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Saves a layer, frame or layerlist state for undo-redo.
        /// </summary>
        /// <param name="eTypeValue">Value of the given <see cref="IPixelEditorEventType"/>.</param>
        /// <param name="selectedLayerIndex">Index of layer or frame to save state for.</param>
        public void SaveState(int eTypeValue, int elementIndex)
        {
            switch (eTypeValue)
            {
                // FrameMemento
                case IPixelEditorEventType.FRAME_MOVE_RIGHT:
                case IPixelEditorEventType.FRAME_MOVE_LEFT:
                case IPixelEditorEventType.FRAME_DUPLICATE:
                case IPixelEditorEventType.FRAME_ADD:
                case IPixelEditorEventType.FRAME_REMOVE:
                    _caretaker.Save(Frames[elementIndex].GetMemento(eTypeValue), FramesCaretakerKey);
                    break;

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
                    indexForLayerListMemento = elementIndex; // GetIndex(ActiveLayer);
                    _caretaker.Save(GetMemento(eTypeValue));
                    break;

                // LayerMemento
                case IPixelEditorEventType.MERGELAYER:
                    var mergeMemento = Layers[elementIndex].GetMemento(eTypeValue);
                    mergeMemento.SetChainedMemento(Layers[elementIndex + 1].GetMemento(IPixelEditorEventType.REMOVELAYER));
                    _caretaker.Save(mergeMemento);
                    break;

                case IPixelEditorEventType.ADDLAYER:
                    _caretaker.Save(Layers[elementIndex].GetMemento(eTypeValue));
                    break;

                default:
                    _caretaker.Save(Layers[elementIndex].GetMemento(eTypeValue));
                    break;
            }
        }

        /// <summary>
        /// Gives restore support for layer undo-redos.
        /// </summary>
        /// <param name="mem">State to restore.</param>
        /// <returns>Memento for redoing the undone operation.</returns>
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
                    origIndex = Layers.FindIndex(x => x.LayerGuid == mem.LayerGuid);
                    if (origIndex != -1)
                    {
                        redoMem = Layers[origIndex].GetMemento(typeValue);
                        Layers[origIndex].Restore(mem);
                        RefreshVisualsThenSignalUpdate();
                        return redoMem;
                    }
                    return null;

                case IPixelEditorEventType.MERGELAYER:
                    origIndex = Layers.FindIndex(x => x.LayerGuid == mem.LayerGuid);
                    if (origIndex != -1)
                    {
                        redoMem = Layers[origIndex].GetMemento(-typeValue);
                        Layers[origIndex].Restore(mem);

                        // Set original pre-merge selected layerindex
                        LayerListChanged?.Invoke(new PixelEditorLayerEventArgs(IPixelEditorEventType.REVERSE_MERGELAYER, origIndex));

                        RefreshVisualsThenSignalUpdate();

                        return redoMem;
                    }
                    return null;

                case IPixelEditorEventType.REVERSE_MERGELAYER:
                    origIndex = Layers.FindIndex(x => x.LayerGuid == mem.LayerGuid);
                    if (origIndex != -1 && Layers.Count >= (origIndex + 2))
                    {
                        redoMem = Layers[origIndex].GetMemento(-typeValue);
                        redoMem.SetChainedMemento(Layers[origIndex + 1].GetMemento(IPixelEditorEventType.REMOVELAYER));
                        MergeLayerDownward(origIndex);
                        return redoMem;
                    }
                    return null;

                case IPixelEditorEventType.ADDLAYER:
                case IPixelEditorEventType.DUPLICATELAYER:
                    redoMem = Layers[mem.LayerIndex].GetMemento(IPixelEditorEventType.REMOVELAYER);
                    RemoveLayerByUndoRedo(mem.LayerIndex);
                    return redoMem;

                case IPixelEditorEventType.REMOVELAYER:
                    originator = new DrawingLayer(this, mem);
                    AddLayerByUndoRedo(originator, mem.LayerIndex);
                    redoMem = Layers[mem.LayerIndex].GetMemento(-typeValue);
                    return redoMem;

                case IPixelEditorEventType.LAYER_INNER_PROPERTY_UPDATE:
                    originator = Layers[mem.LayerIndex];
                    if (originator != null)
                    {
                        redoMem = originator.GetMemento(typeValue);
                        originator.Restore(mem);
                        return redoMem;
                    }
                    return null;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gives restore support for layer-list undo-redos.
        /// </summary>
        /// <param name="mem">State to restore.</param>
        /// <returns>Memento for redoing the undone operation.</returns>
        public IMemento<IPixelEditorEventType> HandleRestore(LayerListMemento mem)
        {
            LayerListMemento redoMem;
            var typeValue = mem.GetMementoType();
            switch (typeValue)
            {
                case IPixelEditorEventType.MOVELAYERUP:
                    indexForLayerListMemento = mem.LayerIndex + 1;
                    redoMem = GetMemento(-typeValue);
                    Restore(mem);
                    return redoMem;

                case IPixelEditorEventType.MOVELAYERDOWN:
                    indexForLayerListMemento = mem.LayerIndex - 1;
                    redoMem = GetMemento(-typeValue);
                    Restore(mem);
                    return redoMem;

                case IPixelEditorEventType.ROTATE:
                case IPixelEditorEventType.ROTATE_ALL:
                case IPixelEditorEventType.ROTATE_COUNTERCLOCKWISE:
                case IPixelEditorEventType.ROTATE_COUNTERCLOCKWISE_ALL:
                    indexForLayerListMemento = ActiveLayerIndex;
                    redoMem = GetMemento(-typeValue);
                    Restore(mem);
                    return redoMem;

                case IPixelEditorEventType.MIRROR_HORIZONTAL:
                case IPixelEditorEventType.MIRROR_HORIZONTAL_ALL:
                case IPixelEditorEventType.MIRROR_VERTICAL:
                case IPixelEditorEventType.MIRROR_VERTICAL_ALL:
                case IPixelEditorEventType.RESIZE:
                    indexForLayerListMemento = ActiveLayerIndex;
                    redoMem = GetMemento(typeValue);
                    Restore(mem);
                    return redoMem;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gives restore support for frame undo-redos.
        /// </summary>
        /// <param name="mem">State to restore.</param>
        /// <returns>Memento for redoing the undone operation.</returns>
        public IMemento<IPixelEditorEventType> HandleRestore(FrameMemento mem)
        {
            FrameMemento redoMem;
            var typeValue = mem.GetMementoType();
            switch (typeValue)
            {
                case IPixelEditorEventType.FRAME_MOVE_LEFT:
                    redoMem = Frames[mem.FrameIndex - 1].GetMemento(-typeValue);
                    MoveDrawingFrameRight(mem.FrameIndex - 1);
                    return redoMem;

                case IPixelEditorEventType.FRAME_MOVE_RIGHT:
                    redoMem = Frames[mem.FrameIndex + 1].GetMemento(-typeValue);
                    MoveDrawingFrameLeft(mem.FrameIndex + 1);
                    return redoMem;

                case IPixelEditorEventType.FRAME_DUPLICATE:
                case IPixelEditorEventType.FRAME_ADD:
                    redoMem = Frames[mem.FrameIndex].GetMemento(IPixelEditorEventType.FRAME_REMOVE);
                    RemoveDrawingFrame(mem.FrameIndex);
                    return redoMem;

                case IPixelEditorEventType.FRAME_REMOVE:
                    ReAddDrawingFrames(mem.Frame, mem.FrameIndex);
                    redoMem = Frames[mem.FrameIndex].GetMemento(-typeValue);
                    return redoMem;

                default:
                    return null;
            }
        }
    }
}