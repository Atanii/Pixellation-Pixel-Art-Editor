using System;
using System.Collections.Generic;

namespace Pixellation.MementoPattern
{
    /// <summary>
    /// Multiton class responsible for organizing and handling the list of saved states for <see cref="Undo()"/> and <see cref="Redo()"/>.
    /// </summary>
    /// <typeparam name="_MementoType">Interface with consts representing the possible reasons that caused <see cref="IMemento{_MementoType}"/> to be saved.</typeparam>
    public class Caretaker<_MementoType> where _MementoType : IMementoType
    {
        /// <summary>
        /// Capacity for saved states (both for undo and redo separately).
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Key for this instance.
        /// </summary>
        public string InstanceKey { get; private set; }

        /// <summary>
        /// Multiton instances.
        /// </summary>
        private static readonly Dictionary<string,Caretaker<_MementoType>> instances = new Dictionary<string, Caretaker<_MementoType>>();

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
        public static event CaretakerEventHandler OnNewUndoAdded;

        /// <summary>
        /// Marks that a new saved state has just been added for redo.
        /// </summary>
        public static event CaretakerEventHandler OnNewRedoAdded;

        /// <summary>
        /// Marks that all undos and redos have just been cleared.
        /// </summary>
        public static event CaretakerEventHandler OnCleared;

        /// <summary>
        /// Marks that an operation has just been undone.
        /// </summary>
        public static event CaretakerEventHandler OnUndone;

        /// <summary>
        /// Marks that an operation has just been redone.
        /// </summary>
        public static event CaretakerEventHandler OnRedone;

        /// <summary>
        /// Marks that an error may have happened.
        /// </summary>
        public static event CaretakerEventHandler OnPossibleError;

        /// <summary>
        /// Class representing <see cref="Caretaker{_MementoType}"/> related exceptions.
        /// </summary>
        public class CaretakerException : Exception
        {
            /// <summary>
            /// Constructor for CaretakerException.
            /// </summary>
            /// <param name="msg">Message describing the error causing this exception.</param>
            public CaretakerException(string msg) : base(msg)
            {
            }
        }

        /// <summary>
        /// Constructor for Caretaker.
        /// </summary>
        /// <param name="key"><see cref="InstanceKey"/>.</param>
        /// <param name="capacity"><see cref="Capacity"/>.</param>
        private Caretaker(string key, int capacity)
        {
            InstanceKey = key;
            Capacity = capacity;
        }


        /// <summary>
        /// Returns an instance for <see cref="Caretaker{_MementoType}"/>.
        /// </summary>
        /// <param name="key"><see cref="InstanceKey"/>, an id for the specific multiton instance.</param>
        /// <param name="capacity"><see cref="Capacity"/>.</param>
        /// <returns>Multiton <see cref="Caretaker{_MementoType}"/> instance.</returns>
        public static Caretaker<_MementoType> GetInstance(string key, int capacity = 100)
        {
            if (key == null || key == string.Empty)
            {
                throw new CaretakerException("Instance key cannot be null or empty string!");
            }
            if (capacity <= 0)
            {
                throw new CaretakerException("Capacity must be a positive number!");
            }
            if (!instances.ContainsKey(key))
            {
                instances.Add(key, new Caretaker<_MementoType>(key, capacity));
            }
            return instances[key];
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
                OnNewUndoAdded?.Invoke(this, new CaretakerEventArgs(this._undoList.Count, this._redoList.Count, InstanceKey));
                _redoList.Clear();
            }
            else
            {
                OnPossibleError?.Invoke(
                    this,
                    new CaretakerEventArgs(
                        this._undoList.Count, this._redoList.Count,
                        InstanceKey,
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
                OnUndone?.Invoke(this, new CaretakerEventArgs(this._undoList.Count, this._redoList.Count, InstanceKey));
                if (redoMem != null)
                {
                    Push(_redoList, redoMem);
                    OnNewRedoAdded?.Invoke(this, new CaretakerEventArgs(this._undoList.Count, this._redoList.Count, InstanceKey));
                }
                else
                {
                    Clear();
                    OnPossibleError?.Invoke(
                        this,
                        new CaretakerEventArgs(
                            this._undoList.Count, this._redoList.Count,
                            InstanceKey,
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
                OnRedone?.Invoke(this, new CaretakerEventArgs(this._undoList.Count, this._redoList.Count, InstanceKey));
                if (undoMem != null)
                {
                    Push(_undoList, undoMem);
                    OnNewUndoAdded?.Invoke(this, new CaretakerEventArgs(this._undoList.Count, this._redoList.Count, InstanceKey));
                }
                else
                {
                    Clear();
                    OnPossibleError?.Invoke(
                        this,
                        new CaretakerEventArgs(
                            this._undoList.Count, this._redoList.Count,
                            InstanceKey,
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
        /// Setting new capacity will cause calling <see cref="Clear"/>.
        /// </summary>
        /// <param name="cap">New capacity. Non-positive numbers will be ignored!</param>
        public void SetCapacity(int cap)
        {
            if (cap > 0)
            {
                Capacity = cap;
                Clear();
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
