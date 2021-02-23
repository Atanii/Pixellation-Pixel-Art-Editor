namespace Pixellation.MementoPattern
{
    /// <summary>
    /// Interface for a class representing a container or helper class for the <see cref="IOriginator{_Memento, _MementoType}"/> classes
    /// in case the restoring itself is better to be done by handler.
    /// </summary>
    /// <typeparam name="_Memento">Object containing the saved state.</typeparam>
    /// <typeparam name="_MementoType">Interface with consts representing the possible reasons that caused <see cref="IMemento{_MementoType}"/> to be saved.</typeparam>
    public interface IOriginatorHandler<_Memento, _MementoType> where _Memento : IMemento<_MementoType> where _MementoType : IMementoType
    {
        /// <summary>
        /// Handles restoring the <see cref="IOriginator{_Memento, _MementoType}"/> to it's original state.
        /// </summary>
        /// <param name="mem">Object representing the saved state.</param>
        /// <returns>In case of <see cref="Caretaker{_MementoType}.Undo"/> it's a saved state for <see cref="Caretaker{_MementoType}.Redo"/>, otherwise the opposite.</returns>
        public IMemento<_MementoType> HandleRestore(_Memento mem);
    }
}