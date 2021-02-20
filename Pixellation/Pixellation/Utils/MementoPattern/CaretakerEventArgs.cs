using System;

namespace Pixellation.Utils.MementoPattern
{
    public class CaretakerEventArgs : EventArgs
    {
        public readonly int RemainingUndos;
        public readonly int RemainingRedos;

        public static new CaretakerEventArgs Empty;

        public CaretakerEventArgs() : base() {}

        public CaretakerEventArgs(int remUndos, int remRedos)
        {
            RemainingUndos = remUndos;
            RemainingRedos = remRedos;
        }
    }
}