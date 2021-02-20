using Pixellation.Utils.MementoPattern;

namespace Pixellation.Components.Editor
{
    public interface IEditorEventType : IMementoType
    {
        public const int ADDLAYER = 1;
        public const int DUPLICATELAYER = 2;
        public const int MERGELAYER = 3;
        public const int REMOVELAYER = 4;

        public const int MOVELAYERUP = 5;
        public const int MOVELAYERDOWN = 6;

        public const int RENAMELAYER = 7;
        public const int LAYER_PIXELS_CHANGED = 8;
        public const int LAYER_OPACITYCHANGED = 9;

        public const int MIRROR_HORIZONTAL = 10;
        public const int MIRROR_HORIZONTAL_ALL = 11;
        public const int MIRROR_VERTICAL = 12;
        public const int MIRROR_VERTICAL_ALL = 13;

        public const int RESIZE = 14;

        public const int ROTATE = 15;
        public const int ROTATE_ALL = 16;
        public const int ROTATE_COUNTERCLOCKWISE = 17;
        public const int ROTATE_COUNTERCLOCKWISE_ALL = 18;
    }
}