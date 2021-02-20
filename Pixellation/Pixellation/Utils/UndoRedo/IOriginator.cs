namespace Pixellation.Utils.MementoPattern
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="_Memento"></typeparam>
    /// <typeparam name="_MementoType"></typeparam>
    public interface IOriginator<_Memento, _MementoType> where _Memento : IMemento<_MementoType> where _MementoType : IMementoType
    {
        public void Restore(_Memento mem);

        public _Memento GetMemento(int mTypeValue);
    }
}