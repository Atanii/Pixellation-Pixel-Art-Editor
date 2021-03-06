namespace Pixellation.Models
{
    /// <summary>
    /// Model representing a layer saved in a json file.
    /// </summary>
    public class LayerModel
    {
        /// <summary>
        /// Bitmap of the layer in base64 stringformat.
        /// </summary>
        public string LayerBitmap { get; set; }

        /// <summary>
        /// Name of the layer.
        /// </summary>
        public string LayerName { get; set; }

        /// <summary>
        /// Opacity of the layer.
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// Indicates if the layer is visible or not.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Width of <see cref="LayerBitmap"/> in pixels.
        /// </summary>
        public int LayerWidth { get; set; }

        /// <summary>
        /// Height of <see cref="LayerBitmap"/> in pixels.
        /// </summary>
        public int LayerHeight { get; set; }

        /// <summary>
        /// Backbuffer size of <see cref="LayerBitmap"/>.
        /// </summary>
        public int LayerStride { get; set; }
    }
}