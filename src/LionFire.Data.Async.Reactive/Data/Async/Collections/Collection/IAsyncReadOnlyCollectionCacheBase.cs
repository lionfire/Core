using DynamicData;
using LionFire.Data.Async.Gets;

namespace LionFire.Data.Collections;

public interface IAsyncReadOnlyCollectionCacheBase<TItem>
    : IGetter<IEnumerable<TItem>>
    , IObservableGetOperations<IEnumerable<TItem>>
    , IReadOnlyCollection<TItem>
{
}

public interface IAsyncReadOnlyCollectionCache<TItem>
    //: IReadOnlyCollection<TItem> // Access via List instead
{
    //DynamicData.IObservableList<TItem> List { get; }
}