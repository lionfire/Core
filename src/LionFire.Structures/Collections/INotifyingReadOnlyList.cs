using System.Collections.Generic;

namespace LionFire.Collections
{

    public interface INotifyingReadOnlyList<ChildType> :
        IReadOnlyList<ChildType>,
        INotifyCollectionChanged<ChildType>
    {
    }
}
