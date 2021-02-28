using Pixellation.Components.Event;
using Pixellation.MementoPattern;
using System;
using System.Collections.Generic;

namespace Pixellation.Utils
{
    using Ctaker = Caretaker<IPixelEditorEventType>;
    using Imemento = IMemento<IPixelEditorEventType>;

    /// <summary>
    /// Class for managing multiple <see cref="Caretaker{_MementoType}"/> for Pixellation.
    /// </summary>
    internal class PixellationCaretakerManager
    {
        private readonly Dictionary<string, Ctaker> _caretakers = new Dictionary<string, Ctaker>();

        private static PixellationCaretakerManager _instance;

        private Ctaker _activeInstance => _caretakers[_activeKey];

        private string _activeKey;

        /// <summary>
        /// Key for active <see cref="Caretaker{_MementoType}"/> instance.
        /// </summary>
        public string ActiveKey
        {
            get
            {
                return _activeKey;
            }
            set
            {
                if (_caretakers.ContainsKey(value))
                {
                    _activeKey = value;
                }
                else
                {
                    throw new CaretakerManagerException("Only keys already added can be set as active!");
                }
            }
        }

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
        /// Exception class for <see cref="PixellationCaretakerManager"/>.
        /// </summary>
        internal class CaretakerManagerException : Exception
        {
            public CaretakerManagerException(string msg) : base(msg)
            {
            }
        }

        /// <summary>
        /// Inits <see cref="PixellationCaretakerManager"/> by routing events to <see cref="Caretaker{_MementoType}"/>.
        /// </summary>
        private PixellationCaretakerManager()
        {
            Ctaker.OnNewUndoAdded += (sender, args) => OnNewUndoAdded?.Invoke(sender, args);
            Ctaker.OnNewRedoAdded += (sender, args) => OnNewRedoAdded?.Invoke(sender, args);
            Ctaker.OnCleared += (sender, args) => OnCleared?.Invoke(sender, args);
            Ctaker.OnUndone += (sender, args) => OnUndone?.Invoke(sender, args);
            Ctaker.OnRedone += (sender, args) => OnRedone?.Invoke(sender, args);
            Ctaker.OnPossibleError += (sender, args) => OnPossibleError?.Invoke(sender, args);
        }

        /// <summary>
        /// Gives the singleton instance for <see cref="PixellationCaretakerManager"/>.
        /// </summary>
        /// <returns>Singleton instance.</returns>
        public static PixellationCaretakerManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PixellationCaretakerManager();
            }
            return _instance;
        }

        /// <summary>
        /// Saves the given memento by default into the active <see cref="Caretaker{_MementoType}"/>.
        /// </summary>
        /// <param name="mem"><see cref="Imemento"/> to save.</param>
        /// <param name="key">Key for <see cref="Caretaker{_MementoType}"/> instance.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Save(Imemento mem, string key = "")
        {
            var res = false;

            if (key != string.Empty)
            {
                if (_caretakers.ContainsKey(key))
                {
                    _caretakers[key].Save(mem);
                    res = true;
                }
                return res;
            }
            res = _activeInstance != null;
            if (res)
            {
                _activeInstance.Save(mem);
            }
            return res;
        }

        /// <summary>
        /// Undos the last operation saved into the active <see cref="Caretaker{_MementoType}"/> instance if no key is provided.
        /// </summary>
        /// <param name="key">Key for <see cref="Caretaker{_MementoType}"/> instance.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Undo(string key = "")
        {
            var res = false;

            if (key != string.Empty)
            {
                if (_caretakers.ContainsKey(key))
                {
                    _caretakers[key].Undo();
                    res = true;
                }
                return res;
            }
            res = _activeInstance != null;
            if (res)
            {
                _activeInstance.Undo();
            }
            return res;
        }

        /// <summary>
        /// Redos the last operation saved into the active <see cref="Caretaker{_MementoType}"/> instance if no key is provided.
        /// </summary>
        /// <param name="key">Key for <see cref="Caretaker{_MementoType}"/> instance.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Redo(string key = "")
        {
            var res = false;

            if (key != string.Empty)
            {
                if (_caretakers.ContainsKey(key))
                {
                    _caretakers[key].Redo();
                    res = true;
                }
                return res;
            }
            res = _activeInstance != null;
            if (res)
            {
                _activeInstance.Redo();
            }
            return res;
        }

        /// <summary>
        /// Clear the undo-redo storage for all available <see cref="Caretaker{_MementoType}"/> instances.
        /// </summary>
        public void ClearAll()
        {
            _caretakers.Clear();
        }

        /// <summary>
        /// Clears the undo-redo storage for the active <see cref="Caretaker{_MementoType}"/> instance if no key is provided.
        /// </summary>
        /// <param name="key">Key for <see cref="Caretaker{_MementoType}"/> instance.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Clear(string key = "")
        {
            var res = false;

            if (key != string.Empty)
            {
                if (_caretakers.ContainsKey(key))
                {
                    _caretakers[key].Clear();
                    res = true;
                }
                return res;
            }
            res = _activeInstance != null;
            if (res)
            {
                _activeInstance.Clear();
            }
            return res;
        }

        /// <summary>
        /// Removes the <see cref="Caretaker{_MementoType}"/> instance determined by the key.
        /// </summary>
        /// <param name="key">Key for <see cref="Caretaker{_MementoType}"/> instance.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemoveCaretaker(string key)
        {
            var res = _caretakers.ContainsKey(key);
            if (res)
            {
                _caretakers.Remove(key);
            }
            return res;
        }

        /// <summary>
        /// Inits a new <see cref="Caretaker{_MementoType}"/> instance.
        /// </summary>
        /// <param name="key">Key for new <see cref="Caretaker{_MementoType}"/> instance.</param>
        /// <param name="capacity">Undo-redo capacity.</param>
        /// <param name="autoActivateIfInitial">Sets the new <see cref="Caretaker{_MementoType}"/> instance as active if true.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool InitCaretaker(string key, int capacity = 100, bool autoActivateIfInitial = true)
        {
            var res = !_caretakers.ContainsKey(key) && capacity > 0;
            if (res)
            {
                _caretakers.Add(key, Ctaker.GetInstance(key, capacity));
                if (autoActivateIfInitial && _caretakers.Count == 1)
                {
                    ActiveKey = key;
                }
            }
            return res;
        }
    }
}