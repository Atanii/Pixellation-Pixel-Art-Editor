namespace Pixellation.MementoPattern
{
    /// <summary>
    /// Represents a saved state for an object.
    /// </summary>
    /// <typeparam name="_MementoType">Interface with consts representing the possible reasons that caused <see cref="IMemento{_MementoType}"/> to be saved.</typeparam>
    public interface IMemento<_MementoType> where _MementoType : IMementoType
    {
        /// <summary>
        /// Get the type of the operation applied after creating this memento.
        /// </summary>
        /// <returns>Value of the corresponding <see cref="_MementoType"/>.</returns>
        public int GetMementoType();

        /// <summary>
        /// Restore saved state.
        /// </summary>
        /// <returns><see cref="IMemento{_MementoType}"/> for <see cref="Caretaker{_MementoType}.Undo"/> or <see cref="Caretaker{_MementoType}.Redo"/>.</returns>
        public IMemento<_MementoType> Restore();
    }
}