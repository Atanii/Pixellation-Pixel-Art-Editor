using Pixellation.Components.Editor;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Pixellation.Interfaces
{
    public interface ILayerManager
    {
        public void SetActiveLayer(int layerIndex = 0, bool signalEvent = false);

        public List<DrawingLayer> Layers { get; }

        public DrawingLayer ActiveLayer { get; }

        public DrawingLayer GetLayer(int layerIndex = 0);

        public void AddLayer(DrawingLayer layer, int layerIndex = 0);

        public void AddLayer(string name, int layerIndex = 0);

        public void DuplicateLayer(int layerIndex = 0);

        public void RemoveLayer(int layerIndex);

        public WriteableBitmap Merge(int from, int to = 0, WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.Alpha);

        public WriteableBitmap GetAllMergedWriteableBitmap();

        public void MoveLayerUp(int layerIndex);

        public void MoveLayerDown(int layerIndex);

        public void MergeLayerDownward(int layerIndex);

        public void Mirror(bool horizontally, bool allLayers = false);

        public void Rotate(bool allLayers = false, bool counterClockWise = false, int angleInDegree = 90);

        public void Resize(int newWidth, int newHeight);

        public void SaveState(int eTypeValue, int layerIndex);

        public void ClearLayer(int layerIndex);

        public int GetActiveLayerIndex();
    }
}