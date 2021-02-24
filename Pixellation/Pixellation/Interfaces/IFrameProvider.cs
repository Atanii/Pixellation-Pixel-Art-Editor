using Pixellation.Components.Editor;
using System.Collections.Generic;

namespace Pixellation.Interfaces
{
    internal interface IFrameProvider
    {
        public List<DrawingFrame> Frames { get; }
        public List<DrawingLayer> Layers { get; }
    }
}