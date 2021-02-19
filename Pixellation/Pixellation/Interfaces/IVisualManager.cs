using Pixellation.Components.Editor;
using Pixellation.Models;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Pixellation.Interfaces
{
    public interface IVisualManager
    {
        public event EventHandler RaiseImageUpdatedEvent;

        public void SetActiveLayer(int layerIndex = 0);

        public List<DrawingLayer> GetLayers();

        public DrawingLayer GetLayer(int layerIndex = 0);

        public int AddLayer(DrawingLayer layer, int layerIndex = 0);

        public int AddLayer(string name, int layerIndex = 0);

        public int DuplicateLayer(int layerIndex = 0);

        public void RemoveLayer(DrawingLayer layer);

        public int RemoveLayer(int layerIndex);

        public WriteableBitmap Merge(int from, int to = 0, WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.Alpha);

        public WriteableBitmap GetAllMergedWriteableBitmap();

        public int MoveLayerUp(int layerIndex);

        public int MoveLayerDown(int layerIndex);

        public int MergeLayerDownward(int layerIndex);

        public List<LayerModel> GetLayerModels();

        public void Mirror(bool horizontally, bool allLayers = false);

        public void Rotate(int angleInDegree, bool allLayers = false, bool counterClockWise = false);

        public void Resize(int newWidth, int newHeight);
    }
}