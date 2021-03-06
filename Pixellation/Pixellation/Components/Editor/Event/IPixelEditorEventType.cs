﻿using Pixellation.MementoPattern;

namespace Pixellation.Components.Event
{
    /// <summary>
    /// Consts representing different types of events that can happen during drawing and editing the image.
    /// </summary>
    public interface IPixelEditorEventType : IMementoType
    {
        // LayerMemento
        public const int ADDLAYER = 1;
        public const int REMOVELAYER = -ADDLAYER;

        public const int DUPLICATELAYER = 2;

        public const int MERGELAYER = 3;
        public const int REVERSE_MERGELAYER = -MERGELAYER;

        public const int LAYER_INNER_PROPERTY_UPDATE = 4;

        public const int LAYER_PIXELS_CHANGED = 5;

        public const int LAYER_OPACITYCHANGED = 6;
        public const int LAYER_VISIBILITYCHANGED = 7;

        // LayerListMemento
        public const int RESIZE = 8;

        public const int MOVELAYERUP = 9;
        public const int MOVELAYERDOWN = -MOVELAYERUP;

        public const int MIRROR_HORIZONTAL = 10;
        public const int MIRROR_HORIZONTAL_ALL = 11;
        public const int MIRROR_VERTICAL = 12;
        public const int MIRROR_VERTICAL_ALL = 13;

        public const int ROTATE = 14;
        public const int ROTATE_COUNTERCLOCKWISE = -ROTATE;
        public const int ROTATE_ALL = 15;
        public const int ROTATE_COUNTERCLOCKWISE_ALL = -ROTATE_ALL;

        public const int CLEAR = 16;

        // Frames
        public const int FRAME_ADD = 17;
        public const int FRAME_DUPLICATE = 18;
        public const int FRAME_REMOVE = -FRAME_ADD;

        public const int FRAME_MERGE_TO_LEFT = 19;
        public const int FRAME_MERGE_REVERSE = -FRAME_MERGE_TO_LEFT;

        public const int FRAME_MOVE_LEFT = -FRAME_MOVE_RIGHT;
        public const int FRAME_MOVE_RIGHT = 20;

        public const int FRAME_NEW_ACTIVE_INDEX = 21;
    }
}