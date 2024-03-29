﻿using LionFire.Data.Collections;


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
