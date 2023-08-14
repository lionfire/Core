using DynamicData;
using LionFire.Data.Async.Gets;

namespace LionFire.Data.Collections;

public interface IEnumerableGetter<TItem>
    : IGetter<IEnumerable<TItem>>
    //, IReadOnlyCollection<TItem> // OLD - use ReadCacheValue
{
}

//public interface IAsyncReadOnlyCollectionCache<TItem>
//    //: IReadOnlyCollection<TItem> // Access via List instead
//{
//    //DynamicData.IObservableList<TItem> List { get; }
//}