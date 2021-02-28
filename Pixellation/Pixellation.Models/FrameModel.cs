using System.Collections.Generic;

namespace Pixellation.Models
{
    /// <summary>
    /// Model representing a frame saved in a json file.
    /// </summary>
    public class FrameModel
    {
        /// <summary>
        /// Name of the frame.
        /// </summary>
        public string FrameName { get; set; }

        /// <summary>
        /// Layers of the frame.
        /// </summary>
        public List<LayerModel> Layers { get; set; }

        /// <summary>
        /// Indicates if the frame is visible.
        /// </summary>
        public bool Visible { get; set; }
    }
}