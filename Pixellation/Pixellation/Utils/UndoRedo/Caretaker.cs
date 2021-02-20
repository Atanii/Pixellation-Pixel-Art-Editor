using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pixellation.Utils.UndoRedo
{
    /// <summary>
    /// Singleton class responsible for organizing and handling the list of saved states for <see cref="Undo()"/> and <see cref="Redo()"/>.
    /// </summary>
    public class Caretaker
    {
        private static Caretaker instance;

        private Stack<IMemento> _undoStack = new Stack<IMemento>();
        private Stack<IMemento> _redoStack = new Stack<IMemento>();

        /// <summary>
        /// Returns an instance for <see cref="Caretaker"/>.
        /// </summary>
        /// <returns>Singleton <see cref="Caretaker"/> instance.</returns>
        public static Caretaker GetInstance()
        {
            if (instance == null)
            {
                instance = new Caretaker();
            }
            return instance;
        }

        /// <summary>
        /// Saves a <see cref="IMemento"/> for posibble undoing.
        /// Clears the list of possible redos!
        /// </summary>
        /// <param name="mem"><see cref="IMemento"/> to be saved.</param>
        public void Save(IMemento mem)
        {
            _undoStack.Push(mem);
            Debug.WriteLine($"Memento added, current count: {_undoStack.Count}");
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
                Debug.WriteLine($"Memento removed, current count: {_undoStack.Count}");
                var redoMem = mem.Restore();
                _redoStack.Push(redoMem);
            }
        }

        /// <summary>
        /// Redos the state before the last undo if another operation wasn't saved since than with <see cref="Save(IMemento)"/>
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var mem = _redoStack.Pop();
                var undoMem = mem.Restore();
                _undoStack.Push(undoMem);
            }
        }

        /// <summary>
        /// Deletes all stored states (both for un- and redo).
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
