﻿using Pixellation.Components.Editor;
using System.Collections.Generic;

namespace Pixellation.Interfaces
{
    public interface IFrameProvider
    {
        public List<DrawingFrame> Frames { get; }
        public List<DrawingLayer> Layers { get; }
        
        public int ActiveFrameIndex { get; }

        public int GetActiveLayerIndex();
    }
}