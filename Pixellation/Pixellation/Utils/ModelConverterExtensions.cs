using Pixellation.Components.Editor;
using Pixellation.Interfaces;
using Pixellation.Models;
using System.Collections.Generic;

namespace Pixellation.Utils
{
    public static class ModelConverterExtensions
    {
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

        public static List<DrawingLayer> ToDrawingLayers(List<LayerModel> models, IDrawingHelper helper)
        {
            var res = new List<DrawingLayer>();
            foreach (var model in models)
            {
                res.Add(model.ToDrawingLayer(helper));
            }
            return res;
        }

        public static List<LayerModel> ToLayerModels(List<DrawingLayer> layers)
        {
            var res = new List<LayerModel>();
            foreach (var layer in layers)
            {
                res.Add(layer.ToLayerModel());
            }
            return res;
        }

        public static FrameModel ToFrameModel(this DrawingFrame frame)
        {
            return new FrameModel
            {
                FrameName = frame.FrameName,
                Opacity = frame.Opacity,
                Visible = frame.Visible,
                Layers = ToLayerModels(frame.Layers)
            };
        }

        public static List<FrameModel> ToFrameModels(List<DrawingFrame> frames)
        {
            var res = new List<FrameModel>();
            foreach (var frame in frames)
            {
                res.Add(frame.ToFrameModel());
            }
            return res;
        }

        public static DrawingFrame ToDrawingFrame(this FrameModel model, IDrawingHelper helper)
        {
            return new DrawingFrame(
                ToDrawingLayers(model.Layers, helper),
                model.FrameName,
                helper,
                model.Visible,
                model.Opacity
            );
        }

        public static List<DrawingFrame> ToDrawingFrames(List<FrameModel> models, IDrawingHelper helper)
        {
            var res = new List<DrawingFrame>();
            foreach (var model in models)
            {
                res.Add(model.ToDrawingFrame(helper));
            }
            return res;
        }

        public static ProjectModel MakeProjectModel(string projectName, List<DrawingFrame> frames)
        {
            return new ProjectModel
            {
                ProjectName = projectName,
                Frames = ToFrameModels(frames)
            };
        }

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