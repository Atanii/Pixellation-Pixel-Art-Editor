using System.Collections.Generic;

namespace Pixellation.Models
{
    internal class ProjectModel
    {
        public List<LayerModel> Layers { get; set; }
        public ProjectDataModel ProjectData { get; set; }
        public string ModelVersion { get; set; }
    }
}