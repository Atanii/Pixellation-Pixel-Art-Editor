namespace Pixellation.Utils.UndoRedo
{
    /// <summary>
    /// Represents a saved state for an object.
    /// </summary>
    public interface IMemento
    {
        /// <summary>
        /// Get the type of the operation applied after creating this memento.
        /// </summary>
        /// <returns>Corresponding <see cref="MementoType"/>.</returns>
        public MementoType GetMementoType();

        /// <summary>
        /// Restore saved state.
        /// </summary>
        /// <returns>Memento for <see cref="Caretaker.Redo()"/>.</returns>
        public IMemento Restore();
    }
}