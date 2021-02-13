using System;

namespace Pixellation.Models
{
    [Serializable()]
    public class LayerModel
    {
        public byte[] LayerBitmap;
        public int Width;
        public int Height;
        public int Stride;
        public string LayerName;
    }
}