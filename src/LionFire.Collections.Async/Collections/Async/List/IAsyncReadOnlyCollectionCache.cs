using DynamicData;
using LionFire.Resolves;

namespace LionFire.Collections.Async;

public interface IAsyncReadOnlyCollectionCache<TItem>
    : IReadOnlyCollection<TItem>
    , IObservableResolves<IEnumerable<TItem>>
    , ILazilyResolves<IEnumerable<TItem>>
{
    //IObservableList<T> ObservableList { get; }  // TODO? I don't see dynamicdata support for going from SourceCache to SourceList interfaces but maybe I'm missing something.
    DynamicData.IObservableList<TItem> List { get; }
}
