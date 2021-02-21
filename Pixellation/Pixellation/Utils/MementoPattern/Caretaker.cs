using System.Collections.Generic;

namespace Pixellation.Utils.MementoPattern
{
    /// <summary>
    /// Singleton class responsible for organizing and handling the list of saved states for <see cref="Undo()"/> and <see cref="Redo()"/>.
    /// </summary>
    /// <typeparam name="_MementoType">Interface with consts representing the possible reasons that caused <see cref="IMemento{_MementoType}"/> to be saved.</typeparam>
    public class Caretaker<_MementoType> where _MementoType : IMementoType
    {
        /// <summary>
        /// Capacity for saved states (both for undo and redo separately). Default capacity is 100.
        /// </summary>
        public int Capacity { get; private set; } = 100;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static Caretaker<_MementoType> instance;

        /// <summary>
        /// LinkedList for saved states that can be undone.
        /// </summary>
        private readonly LinkedList<IMemento<_MementoType>> _undoList = new LinkedList<IMemento<_MementoType>>();

        /// <summary>
        /// LinkedList for saved states that can be redone.
        /// </summary>
        private readonly LinkedList<IMemento<_MementoType>> _redoList = new LinkedList<IMemento<_MementoType>>();

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
        /// Marks that an error may have happened.
        /// </summary>
        public event CaretakerEventHandler OnPossibleError;

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
        /// Saves a <see cref="IMemento{_MementoType}"/> for posibble undoing.
        /// Clears the list of possible redos!
        /// </summary>
        /// <param name="mem"><see cref="IMemento{_MementoType}"/> to be saved.</param>
        public void Save(IMemento<_MementoType> mem)
        {
            if (mem != null)
            {
                Push(_undoList, mem);
                OnNewUndoAdded?.Invoke(this, new CaretakerEventArgs(this._undoList.Count, this._redoList.Count));
                _redoList.Clear();
            }
            else
            {
                OnPossibleError?.Invoke(
                    this,
                    new CaretakerEventArgs(
                        this._undoList.Count, this._redoList.Count,
                        "Memento cannot be null!",
                        CaretakerEventArgs.ErrorType.TRIED_TO_SAVE_NULL
                    )
                );
            }
        }

        /// <summary>
        /// Undos the last state that was stored.
        /// </summary>
        public void Undo()
        {
            if (_undoList.Count > 0)
            {
                var mem = Pop(_undoList);
                var redoMem = mem.Restore();
                OnUndone?.Invoke(this, new CaretakerEventArgs(this._undoList.Count, this._redoList.Count));
                if (redoMem != null)
                {
                    Push(_redoList, redoMem);
                    OnNewRedoAdded?.Invoke(this, new CaretakerEventArgs(this._undoList.Count, this._redoList.Count));
                }
                else
                {
                    Clear();
                    OnPossibleError?.Invoke(
                        this,
                        new CaretakerEventArgs(
                            this._undoList.Count, this._redoList.Count,
                            "Memento cannot be null!",
                            CaretakerEventArgs.ErrorType.NULL_REDO_WAS_GIVEN_TO_STORE
                        )
                    );
                }
            }
        }

        /// <summary>
        /// Redos the state before the last undo if another operation wasn't saved since than with <see cref="Save(IMemento{T})"/>
        /// </summary>
        public void Redo()
        {
            if (_redoList.Count > 0)
            {
                var mem = Pop(_redoList);
                var undoMem = mem.Restore();
                OnRedone?.Invoke(this, new CaretakerEventArgs(this._undoList.Count, this._redoList.Count));
                if (undoMem != null)
                {
                    Push(_undoList, undoMem);
                    OnNewUndoAdded?.Invoke(this, new CaretakerEventArgs(this._undoList.Count, this._redoList.Count));
                }
                else
                {
                    Clear();
                    OnPossibleError?.Invoke(
                        this,
                        new CaretakerEventArgs(
                            this._undoList.Count, this._redoList.Count,
                            "Memento cannot be null!",
                            CaretakerEventArgs.ErrorType.NULL_UNDO_WAS_GIVEN_TO_STORE
                        )
                    );
                }
            }
        }

        /// <summary>
        /// Deletes all stored states (both for un- and redo).
        /// </summary>
        public void Clear()
        {
            _undoList.Clear();
            _redoList.Clear();
            OnCleared?.Invoke(this, CaretakerEventArgs.Empty);
        }

        /// <summary>
        /// Sets a new capacity for this <see cref="Caretaker{_MementoType}"/>.
        /// </summary>
        /// <param name="cap">New capacity. Negative numbers are ignored!</param>
        public void SetCapacity(int cap)
        {
            if (cap > 0)
            {
                Capacity = cap;
            }
        }

        /// <summary>
        /// Pops a memento from the given list.
        /// </summary>
        /// <param name="list"><see cref="LinkedList{IMemento{_MementoType}}"/>.</param>
        /// <returns>Stored memento.</returns>
        private IMemento<_MementoType> Pop(LinkedList<IMemento<_MementoType>> list)
        {
            var tmp = list.Last.Value;
            list.RemoveLast();
            return tmp;
        }

        /// <summary>
        /// Push a new memento into the given list. In case of full capacity, it drops the oldest memento.
        /// </summary>
        /// <param name="list"><see cref="LinkedList{IMemento{_MementoType}}"/>.</param>
        /// <param name="mem">New memento to be stored.</param>
        private void Push(LinkedList<IMemento<_MementoType>> list, IMemento<_MementoType> mem)
        {
            if (list.Count >= Capacity)
            {
                list.RemoveFirst();
            }
            list.AddLast(mem);
        }
    }
}
