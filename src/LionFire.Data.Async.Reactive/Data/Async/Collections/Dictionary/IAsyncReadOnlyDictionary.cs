using DynamicData;

namespace LionFire.Data.Collections;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Suggested interfaces:
///  - ISubscribes<IEnumerable<KeyValuePair<TKey, TItem>>>
/// </remarks>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public interface IAsyncReadOnlyDictionary<TKey, TItem>
    : IEnumerableGetter<KeyValuePair<TKey, TItem>>
    where TKey : notnull
{
    IObservableCache<TItem, TKey> ObservableCache { get; } 

}

