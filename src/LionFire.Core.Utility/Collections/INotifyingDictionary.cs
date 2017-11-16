using System.Collections.Generic;

namespace LionFire.Collections
{
    #endregion

    //////IList, ICollection
    //////INotifyPropertyChanged
    ////, INotifyCollectionChanged
    //public interface INotifyingList<ChildType> : IList<ChildType>, INotifyingList<ChildType>
    //{

    //}

    //public interface INotifyingReadOnlyDictionary<TKey, TValue> :
    //    IReadOnlyDictionary<TKey, TValue>
    //    //, INotifyCollectionChanged<KeyValuePair<TKey, TValue>>
    //    //, INotifyingCollection<TValue>
    //{
    //}

    public interface INotifyingDictionary<TKey, TValue> : IDictionary<TKey, TValue>
#if !AOT
        , IReadOnlyDictionary<TKey, TValue> // TEMP TODO - not really readonly - use duck typing instead
#endif
, INotifyingCollection<TValue>
    //, INotifyCollectionChanged<TValue>
    //, INotifyCollectionChanged<KeyValuePair<TKey, TValue>>
    //, INotifyCollectionChanged // Add this too? And implement in MultiBindableDictionary<>
    {
#if !AOT
        INotifyingDictionary<FilterKey, FilterValue> Filter<FilterKey, FilterValue>();
#endif
        //where TKey : FilterKey
        //where TValue : FilterValue;
    }

}
