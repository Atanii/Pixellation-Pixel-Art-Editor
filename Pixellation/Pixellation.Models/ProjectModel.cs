using System.Collections.Generic;

namespace Pixellation.Models
{
    /// <summary>
    /// Model representing a Pixellation project saved into a json file.
    /// </summary>
    public class ProjectModel
    {
        /// <summary>
        /// Name of the saved project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Frames of the project.
        /// </summary>
        public List<FrameModel> Frames { get; set; }
    }
}