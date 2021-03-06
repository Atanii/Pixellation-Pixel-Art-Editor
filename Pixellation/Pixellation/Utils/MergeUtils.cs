using Pixellation.Components.Editor;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Utils
{
    public class MergeUtils
    {
        /// <summary>
        /// Merges the layers in the given index range into a single WriteableBitmap.
        /// The indexing is reverse!
        /// </summary>
        /// <param name="layers">List of <see cref="DrawingLayer"/> to merge.</param>
        /// <param name="from">From index relative to last layer index</param>
        /// <param name="to">To index relative to last layer index. Default is 0, which means the layer above all others.</param>
        /// <param name="mode">Blending mode for merge.</param>
        /// <returns>The bitmap containing the merged layers. If no merge could have been done in the range, a blank bitmap will be returned.</returns>
        public static WriteableBitmap Merge(List<DrawingLayer> layers, int from, int to = 0, WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.Alpha)
        {
            if (layers.Count == 0)
            {
                return null;
            }

            // Blank bitmap as mergebase
            var merged = BitmapFactory.New(layers[0].Bitmap.PixelWidth, layers[0].Bitmap.PixelHeight);
            merged.Clear(Colors.Transparent);

            var rect = new System.Windows.Rect(0d, 0d, merged.Width, merged.Height);

            for (int i = from; i >= to; i--)
            {
                if (layers[i].Visible)
                    merged.Blit(rect, layers[i].GetWriteableBitmapWithAppliedOpacity(), rect, mode);
            }

            return merged;
        }

        /// <summary>
        /// Merges the layers in the given index range into a single WriteableBitmap.
        /// The indexing is reverse!
        /// </summary>
        /// <param name="layers">List of <see cref="DrawingLayer"/> to merge.</param>
        /// <param name="from">From index relative to last layer index</param>
        /// <param name="to">To index relative to last layer index. Default is 0, which means the layer above all others.</param>
        /// <param name="mode">Blending mode for merge.</param>
        /// <returns>The bitmap containing the merged layers. If no merge could have been done in the range, a blank bitmap will be returned.</returns>
        public static WriteableBitmap MergeAll(List<DrawingLayer> layers, WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.Alpha)
        {
            if (layers.Count == 0)
            {
                return null;
            }

            // Blank bitmap as mergebase
            var merged = BitmapFactory.New(layers[0].Bitmap.PixelWidth, layers[0].Bitmap.PixelHeight);
            merged.Clear(Colors.Transparent);

            var rect = new System.Windows.Rect(0d, 0d, merged.Width, merged.Height);

            for (int i = layers.Count - 1; i >= 0; i--)
            {
                if (layers[i].Visible)
                    merged.Blit(rect, layers[i].GetWriteableBitmapWithAppliedOpacity(), rect, mode);
            }

            return merged;
        }

        /// <summary>
        /// Merges the layers in the given index range into a single WriteableBitmap.
        /// The indexing is reverse!
        /// </summary>
        /// <param name="layers">List of <see cref="DrawingLayer"/> to merge.</param>
        /// <param name="from">From index relative to last layer index</param>
        /// <param name="to">To index relative to last layer index. Default is 0, which means the layer above all others.</param>
        /// <param name="mode">Blending mode for merge.</param>
        /// <returns>The bitmap containing the merged layers. If no merge could have been done in the range, a blank bitmap will be returned.</returns>
        public static WriteableBitmap Merge(List<DrawingFrame> frames, int from, int to = 0, WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.Alpha)
        {
            if (frames.Count == 0)
            {
                return null;
            }

            // Blank bitmap as mergebase
            var merged = BitmapFactory.New(frames[0].Layers[0].Bitmap.PixelWidth, frames[0].Layers[0].Bitmap.PixelHeight);
            merged.Clear(Colors.Transparent);

            var rect = new System.Windows.Rect(0d, 0d, merged.Width, merged.Height);

            for (int i = from; i >= to; i--)
            {
                if (!frames[i].Visible)
                    continue;
                var tmp = MergeAll(frames[i].Layers, mode);
                merged.Blit(rect, tmp, rect, mode);
            }

            return merged;
        }

        /// <summary>
        /// Merges the layers in the given index range into a single WriteableBitmap.
        /// The indexing is reverse!
        /// </summary>
        /// <param name="layers">List of <see cref="DrawingLayer"/> to merge.</param>
        /// <param name="from">From index relative to last layer index</param>
        /// <param name="to">To index relative to last layer index. Default is 0, which means the layer above all others.</param>
        /// <param name="mode">Blending mode for merge.</param>
        /// <returns>The bitmap containing the merged layers. If no merge could have been done in the range, a blank bitmap will be returned.</returns>
        public static WriteableBitmap MergeAll(List<DrawingFrame> frames, WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.Alpha)
        {
            if (frames.Count == 0)
            {
                return null;
            }

            // Blank bitmap as mergebase
            var merged = BitmapFactory.New(frames[0].Layers[0].Bitmap.PixelWidth, frames[0].Layers[0].Bitmap.PixelHeight);
            merged.Clear(Colors.Transparent);

            var rect = new System.Windows.Rect(0d, 0d, merged.Width, merged.Height);

            for (int i = frames.Count - 1; i >= 0; i--)
            {
                if (!frames[i].Visible)
                    continue;
                var tmp = MergeAll(frames[i].Layers, mode);
                merged.Blit(rect, tmp, rect, mode);
            }

            return merged;
        }

        /// <summary>
        /// Merges the layers in the given index range into a single WriteableBitmap.
        /// The indexing is reverse!
        /// </summary>
        /// <param name="bmps">List of <see cref="WriteableBitmap"/> to merge.</param>
        /// <param name="from">From index relative to last layer index</param>
        /// <param name="to">To index relative to last layer index. Default is 0, which means the layer above all others.</param>
        /// <param name="mode">Blending mode for merge.</param>
        /// <returns>The bitmap containing the merged bitmaps. If no merge could have been done in the range, a blank bitmap will be returned.</returns>
        public static WriteableBitmap MergeAll(List<WriteableBitmap> bmps, WriteableBitmapExtensions.BlendMode mode = WriteableBitmapExtensions.BlendMode.Alpha)
        {
            if (bmps.Count == 0)
            {
                return null;
            }

            // Blank bitmap as mergebase
            var merged = BitmapFactory.New(bmps[0].PixelWidth, bmps[0].PixelHeight);
            merged.Clear(Colors.Transparent);

            var rect = new System.Windows.Rect(0d, 0d, merged.Width, merged.Height);

            for (int i = bmps.Count - 1; i >= 0; i--)
            {
                merged.Blit(rect, bmps[i], rect, mode);
            }

            return merged;
        }

        /// <summary>
        /// Generates a spritesheet image from the given frames with the specified numbers or rows and coloumns, using the given color as background and blendmode for merging.
        /// </summary>
        /// <param name="frames">List of frames, each one will be one image on the sheet.</param>
        /// <param name="mode">Blendmode for merging.</param>
        /// <param name="rows">Number of rows in the sheet.</param>
        /// <param name="cols">Number of cols in the sheet.</param>
        /// <param name="backgroundColor">Background color of the image.</param>
        /// <returns>Resulting spritesheet.</returns>
        public static WriteableBitmap GenerateSpriteSheetFromFrames(
            List<DrawingFrame> frames, WriteableBitmapExtensions.BlendMode mode, int rows, int cols, Color backgroundColor)
        {
            if (frames.Count == 0 || rows * cols < frames.Count)
            {
                return null;
            }

            var w = frames[0].Layers[0].Bitmap.PixelWidth;
            var h = frames[0].Layers[0].Bitmap.PixelHeight;

            // Blank bitmap as base
            var merged = BitmapFactory.New(
                w * cols,
                h * rows
            );
            merged.Clear(backgroundColor);

            var srcRect = new System.Windows.Rect(0, 0, w, h);

            for (int row = 0; row < rows; row++)
            {
                Debug.WriteLine($"newline");
                for (int col = 0; col < cols; col++)
                {
                    var rect = new System.Windows.Rect(col * w, row * h, w, h);

                    var index = col + (row * cols);
                    Debug.WriteLine($"{rows}, {cols}, : {index}, {frames.Count}");
                    if (index >= frames.Count)
                    {
                        return merged;
                    }
                    var tmp = MergeAll(frames[index].Layers, mode);

                    merged.Blit(rect, tmp, srcRect, mode);
                }
            }

            return merged;
        }

        /// <summary>
        /// Generates a spritesheet image from the given layers with the specified numbers or rows and coloumns, using the given color as background and blendmode for merging.
        /// </summary>
        /// <param name="layers">List of layers, each one will be one image on the sheet.</param>
        /// <param name="mode">Blendmode for merging.</param>
        /// <param name="rows">Number of rows in the sheet.</param>
        /// <param name="cols">Number of cols in the sheet.</param>
        /// <param name="backgroundColor">Background color of the image.</param>
        /// <returns>Resulting spritesheet.</returns>
        public static WriteableBitmap GenerateSpriteSheetFromLayers(
            List<DrawingLayer> layers, WriteableBitmapExtensions.BlendMode mode, int rows, int cols, Color backgroundColor)
        {
            if (layers.Count == 0 || rows * cols < layers.Count)
            {
                return null;
            }

            var w = layers[0].Bitmap.PixelWidth;
            var h = layers[0].Bitmap.PixelHeight;

            // Blank bitmap as base
            var merged = BitmapFactory.New(
                w * cols,
                h * rows
            );
            merged.Clear(backgroundColor);

            var srcRect = new System.Windows.Rect(0, 0, w, h);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var rect = new System.Windows.Rect(col * w, row * h, w, h);

                    var index = col + (row * cols);
                    if (index >= layers.Count)
                    {
                        return merged;
                    }
                    merged.Blit(rect, layers[index].Bitmap, srcRect, mode);
                }
            }

            return merged;
        }
    }
}