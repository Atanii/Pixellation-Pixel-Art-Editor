namespace Pixellation.Utils.UndoRedo
{
    public interface IOriginatorHandler<T> where T : IMemento
    {
        public T HandleRestore(T mem);
    }
}