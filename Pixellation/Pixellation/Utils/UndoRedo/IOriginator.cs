namespace Pixellation.Utils.UndoRedo
{
    public interface IOriginator<T> where T : IMemento
    {
        public void Restore(T mem);

        public T GetMemento(MementoType mType);
    }
}