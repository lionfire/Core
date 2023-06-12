using DynamicData;
using LionFire.Data.Async.Gets;

namespace LionFire.Collections.Async;

public interface IAsyncReadOnlyCollectionCacheBase<TItem>
    : ILazilyGets<IEnumerable<TItem>>
    , IObservableGets<IEnumerable<TItem>>
    , IReadOnlyCollection<TItem>
{
}

public interface IAsyncReadOnlyCollectionCache<TItem>
    //: IReadOnlyCollection<TItem> // Access via List instead
{
    //DynamicData.IObservableList<TItem> List { get; }
}