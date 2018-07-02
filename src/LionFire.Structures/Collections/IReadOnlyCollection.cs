using System.Collections.Generic;

namespace LionFire.Collections
{
    //    public interface INotifyListChanged<ChildType> FUTURE - for efficient list updates in WPF?
    //{
    //    event NotifyListChangedHandler<ChildType> CollectionChanged;
    //}

    

    // REVIEW - needed?
    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        #region From ICollection

        int Count { get; }
        bool IsReadOnly { get; } // REVIEW - is this needed?
        //bool Contains(T item);
        //void CopyTo(T[//array, int arrayIndex);

        #endregion

        T[] ToArray();
    }

}
