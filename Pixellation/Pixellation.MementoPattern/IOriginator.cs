namespace Pixellation.Utils.MementoPattern
{
    /// <summary>
    /// Represents an object that can return a copy of it's state preceeding an event indicated by <see cref="_MementoType"/>.
    /// </summary>
    /// <typeparam name="_Memento">Object containing the saved state.</typeparam>
    /// <typeparam name="_MementoType">Interface with consts representing the possible reasons that caused <see cref="IMemento{_MementoType}"/> to be saved.</typeparam>
    public interface IOriginator<_Memento, _MementoType> where _Memento : IMemento<_MementoType> where _MementoType : IMementoType
    {
        /// <summary>
        /// Restore saved state with or without the help of a <see cref="IOriginatorHandler{T, O}"/>.
        /// </summary>
        /// <param name="mem">Object containing a saved state.</param>
        public void Restore(_Memento mem);

        /// <summary>
        /// Returns a <see cref="_Memento"/> containing the saved state.
        /// </summary>
        /// <param name="mTypeValue">Value indicating the type of operation that will be commited after getting this saved state.</param>
        /// <returns>Object containing the saved state.</returns>
        public _Memento GetMemento(int mTypeValue);
    }
}