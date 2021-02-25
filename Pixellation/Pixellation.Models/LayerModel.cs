using System;

namespace Pixellation.Models
{
    public class LayerModel
    {
        public string LayerBitmap { get; set; }
        public string LayerName { get; set; }
        public double Opacity { get; set; }
        public bool Visible { get; set; }
        public int LayerWidth { get; set; }
        public int LayerHeight { get; set; }
        public int LayerStride { get; set; }
    }
}