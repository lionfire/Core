using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace LionFire.Collections
{

    public interface INotifyingCollection<T> : ICollection<T>, INotifyCollectionChanged<T>
#if AOT
, INotifyCollectionChanged
#endif
    {
        T[] ToArray();
    }

}
