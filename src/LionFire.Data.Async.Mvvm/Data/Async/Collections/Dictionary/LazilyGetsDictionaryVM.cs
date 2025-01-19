#if false
using LionFire.Data.Collections;


namespace LionFire.Data.Mvvm;

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

    public IAsyncReadOnlyDictionary<TKey, TValue>? Cache => Source as IAsyncReadOnlyDictionary<TKey, TValue>;
    public IObservableCache<TValue, TKey>? ObservableCache => Cache?.ObservableCache;

    #endregion
}


#if TODO // maybe
// Without TCollection
public class LazilyGetsKeyedCollectionVM<TKey, TValue>
    : LazilyGetsKeyedCollectionVM<TKey, TValue, IEnumerable<KeyValuePair<TKey, TValue>>>
    where TKey : notnull
{
    #region Lifecycle

    public LazilyGetsKeyedCollectionVM(IViewModelProvider viewModelProvider, Func<TValue, TKey> keySelector) : base(viewModelProvider)
    {
    }

    #endregion
}

// TODO: TValueVM
public class LazilyGetsKeyedCollectionVM<TKey, TValue, TCollection>
    : LazilyGetsCollectionVM<KeyValuePair<TKey, TValue>, TCollection>
    where TKey : notnull
    where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
{
    #region Lifecycle

    public LazilyGetsKeyedCollectionVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion

    #region State (derived)

    public IAsyncReadOnlyKeyedCollection<TKey, TValue>? Cache => Source as IAsyncReadOnlyKeyedCollection<TKey, TValue>;
    public IObservableCache<TValue, TKey>? ObservableCache => Cache?.ObservableCache;

    #endregion
}

#endif
#endif