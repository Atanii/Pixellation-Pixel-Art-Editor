using System;

namespace Pixellation.Utils.MementoPattern
{
    /// <summary>
    /// Information describing the state of the <see cref="Caretaker{_MementoType}"/> when the event happened.
    /// </summary>
    public class CaretakerEventArgs : EventArgs
    {
        /// <summary>
        /// Enum describing an error.
        /// </summary>
        public enum ErrorType
        {
            /// <summary>
            /// There was no error.
            /// </summary>
            NO_ERROR,
            /// <summary>
            /// The program tried to save a given <see cref="IMemento{_MementoType}"/> instance that was null!
            /// </summary>
            TRIED_TO_SAVE_NULL,
            /// <summary>
            /// The program tried to store a given <see cref="IMemento{_MementoType}"/> instance for undo that was null!
            /// </summary>
            NULL_UNDO_WAS_GIVEN_TO_STORE,
            /// <summary>
            /// The program tried to store a given <see cref="IMemento{_MementoType}"/> instance for redo that was null!
            /// </summary>
            NULL_REDO_WAS_GIVEN_TO_STORE
        }

        /// <summary>
        /// Error information.
        /// </summary>
        public readonly ErrorType ErrorState;

        /// <summary>
        /// How many <see cref="IMemento{_MementoType}"/> objects are saved at the moment for <see cref="Caretaker{_MementoType}.Undo"/>.
        /// </summary>
        public readonly int RemainingUndos;

        /// <summary>
        /// How many <see cref="IMemento{_MementoType}"/> objects are saved at the moment for <see cref="Caretaker{_MementoType}.Redo"/>.
        /// </summary>
        public readonly int RemainingRedos;

        /// <summary>
        /// Extra information about the event.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Empty args without information.
        /// </summary>
        public static new CaretakerEventArgs Empty;

        /// <summary>
        /// Creates an empty <see cref="CaretakerEventArgs"/>.
        /// </summary>
        public CaretakerEventArgs() : base() {}

        /// <summary>
        /// Creates an <see cref="CaretakerEventArgs"/> with information about the current amount of saved operations and maybe about the event or a possible error.
        /// </summary>
        /// <param name="remUndos"><see cref="RemainingUndos"/>.</param>
        /// <param name="remRedos"><see cref="RemainingRedos"/>.</param>
        /// <param name="msg">Optional <see cref="Message"/>.</param>
        /// <param name="errorType">State describing the error that happened.</param>
        public CaretakerEventArgs(int remUndos, int remRedos, string msg = "", ErrorType errorType = ErrorType.NO_ERROR)
        {
            RemainingUndos = remUndos;
            RemainingRedos = remRedos;
            Message = msg;
            ErrorState = errorType;
        }
    }
}