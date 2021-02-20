namespace Pixellation.Utils.MementoPattern
{
    public interface IOriginatorHandler<T, O> where T : IMemento<O> where O : IMementoType
    {
        public IMemento<O> HandleRestore(T mem);
    }
}