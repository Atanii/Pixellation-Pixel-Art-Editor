using Pixellation.Components.Editor;
using Pixellation.Interfaces;
using Pixellation.Models;
using System.Collections.Generic;

namespace Pixellation.Utils
{
    /// <summary>
    /// Helper class for converting to and from to filemodels.
    /// </summary>
    public static class ModelConverterExtensions
    {
        /// <summary>
        /// Converts a model to a layer.
        /// </summary>
        /// <param name="layer">Layer to convert.</param>
        /// <param name="helper"><see cref="IDrawingHelper"/> for initializing the layer.</param>
        /// <returns>Resulting layer.</returns>
        public static DrawingLayer ToDrawingLayer(this LayerModel layer, IDrawingHelper helper)
        {
            return new DrawingLayer(
                helper,
                layer.LayerBitmap.ToWriteableBitmap(
                    layer.LayerWidth,
                    layer.LayerHeight,
                    layer.LayerStride
                    ),
                layer.LayerName,
                layer.Visible,
                layer.Opacity
            );
        }

        /// <summary>
        /// Converts a layer to a model.
        /// </summary>
        /// <param name="layer">Layer to convert.</param>
        /// <returns>Resulting model.</returns>
        public static LayerModel ToLayerModel(this DrawingLayer layer)
        {
            return new LayerModel
            {
                LayerBitmap = layer.Bitmap.ToBase64(),
                LayerHeight = layer.Bitmap.PixelHeight,
                LayerWidth = layer.Bitmap.PixelWidth,
                LayerName = layer.LayerName,
                LayerStride = layer.Bitmap.BackBufferStride,
                Opacity = layer.Opacity,
                Visible = layer.Visible
            };
        }

        /// <summary>
        /// Converts a list of models to a list of layers.
        /// </summary>
        /// <param name="models">Models to convert.</param>
        /// <param name="helper"><see cref="IDrawingHelper"/> for intializing layers.</param>
        /// <returns>Resulting list of layers.</returns>
        public static List<DrawingLayer> ToDrawingLayers(List<LayerModel> models, IDrawingHelper helper)
        {
            var res = new List<DrawingLayer>();
            foreach (var model in models)
            {
                res.Add(model.ToDrawingLayer(helper));
            }
            return res;
        }

        /// <summary>
        /// Converts a list of layers to a list of models.
        /// </summary>
        /// <param name="layers">List of layers to convert.</param>
        /// <returns>Resulting list of models.</returns>
        public static List<LayerModel> ToLayerModels(List<DrawingLayer> layers)
        {
            var res = new List<LayerModel>();
            foreach (var layer in layers)
            {
                res.Add(layer.ToLayerModel());
            }
            return res;
        }

        /// <summary>
        /// Converts a frame to a model.
        /// </summary>
        /// <param name="frame">Frame to convert.</param>
        /// <returns>Resulting model.</returns>
        public static FrameModel ToFrameModel(this DrawingFrame frame)
        {
            return new FrameModel
            {
                FrameName = frame.FrameName,
                Visible = frame.Visible,
                Layers = ToLayerModels(frame.Layers)
            };
        }

        /// <summary>
        /// Converts a list of frames to a list of models.
        /// </summary>
        /// <param name="frames">Frames to convert.</param>
        /// <returns>Resulting list of models.</returns>
        public static List<FrameModel> ToFrameModels(List<DrawingFrame> frames)
        {
            var res = new List<FrameModel>();
            foreach (var frame in frames)
            {
                res.Add(frame.ToFrameModel());
            }
            return res;
        }

        /// <summary>
        /// Converts a model to a frame.
        /// </summary>
        /// <param name="model">Model to convert.</param>
        /// <param name="helper"><see cref="IDrawingHelper"/> for initializing the frame.</param>
        /// <returns>Resulting frame.</returns>
        public static DrawingFrame ToDrawingFrame(this FrameModel model, IDrawingHelper helper)
        {
            return new DrawingFrame(
                ToDrawingLayers(model.Layers, helper),
                model.FrameName,
                helper,
                model.Visible
            );
        }

        /// <summary>
        /// Converts a list of models to a list of frames.
        /// </summary>
        /// <param name="models">Models to convert.</param>
        /// <param name="helper"><see cref="IDrawingHelper"/> for initializing frames.</param>
        /// <returns>Resulting list of frames.</returns>
        public static List<DrawingFrame> ToDrawingFrames(List<FrameModel> models, IDrawingHelper helper)
        {
            var res = new List<DrawingFrame>();
            foreach (var model in models)
            {
                res.Add(model.ToDrawingFrame(helper));
            }
            return res;
        }

        /// <summary>
        /// Generates a model from a projectname and a list of frames.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="frames">Frames.</param>
        /// <returns>Resulting model.</returns>
        public static ProjectModel MakeProjectModel(string projectName, List<DrawingFrame> frames)
        {
            return new ProjectModel
            {
                ProjectName = projectName,
                Frames = ToFrameModels(frames)
            };
        }

        /// <summary>
        /// Converts a model into actual projectdata.
        /// </summary>
        /// <param name="pm">Model to convert.</param>
        /// <param name="helper"><see cref="IDrawingHelper"/> for initializing frames and layers.</param>
        /// <returns>Projectname as key and list of frames as value.</returns>
        public static KeyValuePair<string, List<DrawingFrame>> GetProjectData(this ProjectModel pm, IDrawingHelper helper)
        {
            return new KeyValuePair<string, List<DrawingFrame>>
            (
                pm.ProjectName,
                ToDrawingFrames(pm.Frames, helper)
            );
        }
    }
}