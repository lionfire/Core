using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace LionFire.Collections
{

    #region Collection Interfaces

    public interface INotifyingCollection<T> : ICollection<T>, INotifyCollectionChanged<T>
#if AOT
, INotifyCollectionChanged
#endif
    {
        T[] ToArray();
    }

    #endregion

    #region List Interfaces

    public interface IReadOnlyList<T> : IReadOnlyCollection<T>
    {
        T this[int index] { get; }
    }

    public interface INotifyingReadOnlyList<ChildType> :
        IReadOnlyList<ChildType>,
        INotifyCollectionChanged<ChildType>
    {
    }

#endregion

}
