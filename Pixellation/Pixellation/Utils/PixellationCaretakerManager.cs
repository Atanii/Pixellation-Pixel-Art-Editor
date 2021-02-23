using Pixellation.Components.Editor;
using Pixellation.MementoPattern;
using System;
using System.Collections.Generic;

namespace Pixellation.Utils
{
    using Ctaker = Caretaker<IPixelEditorEventType>;
    using Imemento = IMemento<IPixelEditorEventType>;

    internal class PixellationCaretakerManager
    {
        private readonly Dictionary<string, Ctaker> _caretakers = new Dictionary<string, Ctaker>();

        private static PixellationCaretakerManager _instance;

        private Ctaker _activeInstance => _caretakers[_activeKey];

        private string _activeKey;

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

        internal class CaretakerManagerException : Exception
        {
            public CaretakerManagerException(string msg) : base(msg)
            {
            }
        }

        private PixellationCaretakerManager()
        {
            Ctaker.OnNewUndoAdded += OnNewUndoAdded;
            Ctaker.OnNewRedoAdded += OnNewRedoAdded;
            Ctaker.OnCleared += OnCleared;
            Ctaker.OnUndone += OnUndone;
            Ctaker.OnRedone += OnRedone;
            Ctaker.OnPossibleError += OnPossibleError;
        }

        public static PixellationCaretakerManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PixellationCaretakerManager();
            }
            return _instance;
        }

        public bool Save(Imemento mem)
        {
            var res = _activeInstance != null;
            if (res)
            {
                _activeInstance.Save(mem);
            }
            return res;
        }

        public bool Undo()
        {
            var res = _activeInstance != null;
            if (res)
            {
                _activeInstance.Undo();
            }
            return res;
        }

        public bool Redo()
        {
            var res = _activeInstance != null;
            if (res)
            {
                _activeInstance.Redo();
            }
            return res;
        }

        public void ClearAll()
        {
            _caretakers.Clear();
        }

        public bool Clear()
        {
            var res = _activeInstance != null;
            if (res)
            {
                _activeInstance.Clear();
            }
            return res;
        }

        public bool RemoveCaretaker(string key, bool clearBeforeRemove = false)
        {
            var res = _caretakers.ContainsKey(key);
            if (res)
            {
                if (clearBeforeRemove)
                {
                    _caretakers[key].Clear();
                }
                _caretakers.Remove(key);
            }
            return res;
        }

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