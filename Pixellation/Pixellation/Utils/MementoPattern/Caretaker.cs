using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pixellation.Utils.MementoPattern
{
    /// <summary>
    /// Singleton class responsible for organizing and handling the list of saved states for <see cref="Undo()"/> and <see cref="Redo()"/>.
    /// </summary>
    /// <typeparam name="_MementoType">Type implemeting <see cref="IMementoType"/> that indicates what kind of event preceeded the save.</typeparam>
    public class Caretaker<_MementoType> where _MementoType : IMementoType
    {
        private static Caretaker<_MementoType> instance;

        private readonly Stack<IMemento<_MementoType>> _undoStack = new Stack<IMemento<_MementoType>>();
        private readonly Stack<IMemento<_MementoType>> _redoStack = new Stack<IMemento<_MementoType>>();

        /// <summary>
        /// Marks that a new saved state has just been added for undo.
        /// </summary>
        public event CaretakerEventHandler OnNewUndoAdded;

        /// <summary>
        /// Marks that a new saved state has just been added for redo.
        /// </summary>
        public event CaretakerEventHandler OnNewRedoAdded;

        /// <summary>
        /// Marks that all undos and redos have just been cleared.
        /// </summary>
        public event CaretakerEventHandler OnCleared;

        /// <summary>
        /// Marks that an operation has just been undone.
        /// </summary>
        public event CaretakerEventHandler OnUndone;

        /// <summary>
        /// Marks that an operation has just been redone.
        /// </summary>
        public event CaretakerEventHandler OnRedone;

        /// <summary>
        /// Returns an instance for <see cref="Caretaker{_MementoType}"/>.
        /// </summary>
        /// <returns>Singleton <see cref="Caretaker{_MementoType}"/> instance.</returns>
        public static Caretaker<_MementoType> GetInstance()
        {
            if (instance == null)
            {
                instance = new Caretaker<_MementoType>();
            }
            return instance;
        }

        /// <summary>
        /// Saves a <see cref="IMemento{T}"/> for posibble undoing.
        /// Clears the list of possible redos!
        /// </summary>
        /// <param name="mem"><see cref="IMemento{T}"/> to be saved.</param>
        public void Save(IMemento<_MementoType> mem)
        {
            _undoStack.Push(mem);
            OnNewUndoAdded?.Invoke(this, new CaretakerEventArgs(this._undoStack.Count, this._redoStack.Count));
            _redoStack.Clear();
        }

        /// <summary>
        /// Undos the last state that was stored.
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var mem = _undoStack.Pop();
                var redoMem = mem.Restore();
                OnUndone?.Invoke(this, new CaretakerEventArgs(this._undoStack.Count, this._redoStack.Count));
                if (redoMem != null)
                {
                    _redoStack.Push(redoMem);
                    OnNewRedoAdded?.Invoke(this, new CaretakerEventArgs(this._undoStack.Count, this._redoStack.Count));
                }
                else
                {
                    Clear();
                }
            }
        }

        /// <summary>
        /// Redos the state before the last undo if another operation wasn't saved since than with <see cref="Save(IMemento{T})"/>
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var mem = _redoStack.Pop();
                var undoMem = mem.Restore();
                OnRedone?.Invoke(this, new CaretakerEventArgs(this._undoStack.Count, this._redoStack.Count));
                if (undoMem != null)
                {
                    _undoStack.Push(undoMem);
                    OnNewUndoAdded?.Invoke(this, new CaretakerEventArgs(this._undoStack.Count, this._redoStack.Count));
                }
                else
                {
                    Clear();
                }
            }
        }

        /// <summary>
        /// Deletes all stored states (both for un- and redo).
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            OnCleared?.Invoke(this, CaretakerEventArgs.Empty);
        }
    }
}
