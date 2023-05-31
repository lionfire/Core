using LionFire.Collections.Async;

namespace LionFire.Mvvm;

// Without TCollection
public class LazilyResolvesDictionaryVM<TKey, TValue>
    : LazilyResolvesDictionaryVM<TKey, TValue, IEnumerable<KeyValuePair<TKey, TValue>>>
    where TKey : notnull
{
    #region Lifecycle

    public LazilyResolvesDictionaryVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion
}


// TODO: TValueVM
public class LazilyResolvesDictionaryVM<TKey, TValue, TCollection>
    : LazilyResolvesCollectionVM<KeyValuePair<TKey, TValue>, TCollection>
    where TKey : notnull
    where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
{
    #region Lifecycle

    public LazilyResolvesDictionaryVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion

    #region State (derived)

    public IAsyncReadOnlyDictionaryCache<TKey, TValue>? Cache => Source as IAsyncReadOnlyDictionaryCache<TKey, TValue>;
    public IObservableCache<TValue, TKey>? ObservableCache => Cache?.ObservableCache;

    #endregion
}
