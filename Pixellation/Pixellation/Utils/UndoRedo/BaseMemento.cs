namespace Pixellation.Utils.MementoPattern
{
    /// <summary>
    /// Base memento class storing the corresponding <see cref="_MementoType"/> and <see cref="IOriginator{_Memento, _MementoType}"/>.
    /// </summary>
    /// <typeparam name="_Memento">Class inherited from <see cref="BaseMemento{_Memento, _MementoType}"/>.</typeparam>
    public abstract class BaseMemento<_Memento, _MementoType> : IMemento<_MementoType> where _Memento : IMemento<_MementoType> where _MementoType : IMementoType
    {
        /// <summary>
        /// Value of the <see cref="_MementoType"/> indicating the change preceeding the creation of this <see cref="_Memento"/>.
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
        /// Override this method to call <see cref="IOriginatorHandler{T, O}.HandleRestore(T)"/> from the inherited class.
        /// </summary>
        /// <returns><see cref="IMemento"/>.</returns>
        public abstract IMemento<_MementoType> Restore();

        /// <summary>
        /// Returns the value of the <see cref="_MementoType"/> for this <see cref="BaseMemento{_Memento, _MementoType}"/>.
        /// </summary>
        /// <returns></returns>
        public int GetMementoType() => _mTypeValue;
    }
}