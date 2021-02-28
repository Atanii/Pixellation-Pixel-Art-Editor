using Pixellation.Components.Editor;
using Pixellation.Components.Editor.Memento;
using Pixellation.MementoPattern;

namespace Pixellation.Interfaces
{
    public interface IDrawingHelper : 
        IOriginatorHandler<LayerMemento, IPixelEditorEventType>,
        IOriginatorHandler<LayerListMemento, IPixelEditorEventType>,
        IOriginatorHandler<FrameMemento, IPixelEditorEventType>
    {
        int Magnification { get; }

        int PixelWidth { get; }

        int PixelHeight { get; }

        bool TiledModeOn { get; }

        float TiledOpacity { get; }

        string ActiveFrameId { get; }

        public int ActiveFrameIndex { get; }

        public void SetActiveFrame(DrawingFrame frame);

        public int GetIndex(DrawingLayer layer);

        public void RefreshVisualsThenSignalUpdate();

        public void SaveState(int eTypeValue, int selectedLayerIndex);

        public int ActiveLayerIndex { get; }
    }
}