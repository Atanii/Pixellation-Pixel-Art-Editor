using System.Collections.Generic;

namespace Pixellation.Models
{
    public class ProjectModel
    {
        public string ProjectName { get; set; }
        public List<FrameModel> Frames { get; set; }
    }
}