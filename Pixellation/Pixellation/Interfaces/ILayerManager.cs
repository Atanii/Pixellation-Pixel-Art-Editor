using Pixellation.Components.Editor;
using System.Collections.Generic;

namespace Pixellation.Interfaces
{
    /// <summary>
    /// Provieds helping functionality for layer lists and similar components.
    /// </summary>
    public interface ILayerManager
    {
        /// <summary>
        /// Sets selected layer.
        /// </summary>
        /// <param name="layerIndex">Index of layers to be selected.</param>
        public void SetActiveLayer(int layerIndex = 0);

        /// <summary>
        /// Currently edited layers.
        /// </summary>
        public List<DrawingLayer> Layers { get; }

        /// <summary>
        /// Currently selected layer.
        /// </summary>
        public DrawingLayer ActiveLayer { get; }

        /// <summary>
        /// Adds a new layer.
        /// </summary>
        /// <param name="layer">Layer to be added.</param>
        /// <param name="layerIndex">Index of the new layer.</param>
        public void AddLayer(DrawingLayer layer, int layerIndex = 0);

        /// <summary>
        /// Adds a new layer.
        /// </summary>
        /// <param name="name">Name of the new layer.</param>
        /// <param name="layerIndex">Index of the new layer.</param>
        public void AddLayer(string name, int layerIndex = 0);

        /// <summary>
        /// Duplicates a layer.
        /// </summary>
        /// <param name="layerIndex">Index of the layer to duplicate.</param>
        public void DuplicateLayer(int layerIndex = 0);

        /// <summary>
        /// Removes a layer.
        /// </summary>
        /// <param name="layerIndex">Index of the layer to remove.</param>
        public void RemoveLayer(int layerIndex);

        /// <summary>
        /// Moves a layer up (closer).
        /// </summary>
        /// <param name="layerIndex">Index of layer to move.</param>
        public void MoveLayerUp(int layerIndex);

        /// <summary>
        /// Moves a layer down (behind).
        /// </summary>
        /// <param name="layerIndex">Index of layer to move.</param>
        public void MoveLayerDown(int layerIndex);

        /// <summary>
        /// Merges the layer behind the selected one into the selected.
        /// </summary>
        /// <param name="layerIndex">Index of layer selected for merge.</param>
        public void MergeLayerDownward(int layerIndex);

        /// <summary>
        /// Mirrors drawn image.
        /// </summary>
        /// <param name="horizontally">Mirror horizontally or vertically.</param>
        /// <param name="allLayers">Only selected or all layers in the frame.</param>
        public void Mirror(bool horizontally, bool allLayers = false);

        /// <summary>
        /// Rotates the image, applies to all image.
        /// </summary>
        /// <param name="counterClockWise">Counter- or clovkwise rotation.</param>
        public void Rotate(bool counterClockWise = false);

        /// <summary>
        /// Resizes edited image.
        /// </summary>
        /// <param name="newWidth">New width in pixels.</param>
        /// <param name="newHeight">New height in pixels.</param>
        public void Resize(int newWidth, int newHeight);

        /// <summary>
        /// Saves layerstate for possible undo.
        /// </summary>
        /// <param name="eTypeValue">Value of <see cref="IPixelEditorEventType"/>.</param>
        /// <param name="layerIndex">Index of layer to save.</param>
        public void SaveState(int eTypeValue, int layerIndex);

        /// <summary>
        /// Clears the layer leaving only transparent pixels.
        /// </summary>
        /// <param name="layerIndex">Index of layer to clear.</param>
        public void ClearLayer(int layerIndex);

        /// <summary>
        /// Index of the selected layer.
        /// </summary>
        public int ActiveLayerIndex { get; }
    }
}