using System.Collections.Generic;

namespace LionFire.Collections
{
    public interface INotifyingList<ChildType> : // RENAME to eliminate Modifible, rename INotifyingCollection to INotifyingReadonlyCollection
                                                 //SNotifyingCollection<ChildType>,
        INotifyingCollection<ChildType>,
        //INotifyingReadOnlyList<ChildType>,  //RECENTCHANGE - Commetned this
        INotifyCollectionChanged<ChildType>, //RECENTCHANGE - Added this
        IList<ChildType>
    {
#if !AOT
        INotifyingList<FilterType> Filter<FilterType>();
#endif
        //where ChildType : FilterType;
    }

}
