using Pixellation.Components.Editor;

namespace Pixellation.Utils.MementoPattern
{
    /// <summary>
    /// Represents a saved state for an object.
    /// </summary>
    /// <typeparam name="_MementoType">Type implemeting <see cref="IMementoType"/> that indicates what kind of event preceeded the save.</typeparam>
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
        /// <returns><see cref="IMemento{_MementoType}"/> for <see cref="Caretaker.Redo()"/>.</returns>
        public IMemento<_MementoType> Restore();
    }
}