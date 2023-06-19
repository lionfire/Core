using LionFire.Data.Async.Collections;


namespace LionFire.Data.Async.Mvvm;

// Without TCollection
public class LazilyGetsDictionaryVM<TKey, TValue>
    : LazilyGetsDictionaryVM<TKey, TValue, IEnumerable<KeyValuePair<TKey, TValue>>>
    where TKey : notnull
{
    #region Lifecycle

    public LazilyGetsDictionaryVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion
}


// TODO: TValueVM
public class LazilyGetsDictionaryVM<TKey, TValue, TCollection>
    : LazilyGetsCollectionVM<KeyValuePair<TKey, TValue>, TCollection>
    where TKey : notnull
    where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
{
    #region Lifecycle

    public LazilyGetsDictionaryVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion

    #region State (derived)

    public IAsyncReadOnlyDictionaryCache<TKey, TValue>? Cache => Source as IAsyncReadOnlyDictionaryCache<TKey, TValue>;
    public IObservableCache<TValue, TKey>? ObservableCache => Cache?.ObservableCache;

    #endregion
}
