using System.Collections.Generic;

namespace Pixellation.Models
{
    public class FrameModel
    {
        public string FrameName { get; set; }
        public List<LayerModel> Layers { get; set; }
        public double Opacity { get; set; }
        public bool Visible { get; set; }
    }
}