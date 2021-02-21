namespace Pixellation.Utils.MementoPattern
{
    /// <summary>
    /// Base memento class storing the corresponding <see cref="_MementoType"/> and <see cref="IOriginator{_Memento, _MementoType}"/>.
    /// </summary>
    /// <typeparam name="_Memento">Object containing the saved state.</typeparam>
    /// <typeparam name="_MementoType">Interface with consts representing the possible reasons that caused <see cref="IMemento{_MementoType}"/> to be saved.</typeparam>
    public abstract class BaseMemento<_Memento, _MementoType> : IMemento<_MementoType> where _Memento : IMemento<_MementoType> where _MementoType : IMementoType
    {
        /// <summary>
        /// Value of the <see cref="_MementoType"/> indicating the change following the creation of this <see cref="_Memento"/>.
        /// </summary>
        protected readonly int _mTypeValue;

        /// <summary>
        /// <see cref="IOriginator{_Memento, _MementoType}"/> to handle restore for this <see cref="_Memento"/>.
        /// </summary>
        protected readonly IOriginatorHandler<_Memento, _MementoType> _originatorHandler;

        /// <summary>
        /// Initializes a <see cref="BaseMemento{_Memento, _MementoType}"/> instance.
        /// </summary>
        /// <param name="mTypeValue">Value of the corresponding <see cref="_MementoType"/>.</param>
        /// <param name="originatorHandler"><see cref="IOriginator{_Memento, _MementoType}"/> to handle restore for this <see cref="_Memento"/>.</param>
        public BaseMemento(int mTypeValue, IOriginatorHandler<_Memento, _MementoType> originatorHandler)
        {
            _mTypeValue = mTypeValue;
            _originatorHandler = originatorHandler;
        }

        /// <summary>
        /// Restore saved state.
        /// </summary>
        /// <returns><see cref="IMemento{_MementoType}"/> for <see cref="Caretaker{_MementoType}.Undo"/> or <see cref="Caretaker{_MementoType}.Redo"/>.</returns>
        public abstract IMemento<_MementoType> Restore();

        /// <summary>
        /// Returns the value of the <see cref="_MementoType"/> for this <see cref="BaseMemento{_Memento, _MementoType}"/>.
        /// </summary>
        /// <returns>Value of the <see cref="_MementoType"/>.</returns>
        public int GetMementoType() => _mTypeValue;
    }
}