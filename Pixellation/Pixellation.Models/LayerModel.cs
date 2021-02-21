using System;

namespace Pixellation.Models
{
    [Serializable()]
    public class LayerModel
    {
        public byte[] LayerBitmap { get; set; }
        public string LayerName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Stride { get; set; }
        public double Opacity { get; set; }
    }
}