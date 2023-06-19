﻿using LionFire.Mvvm;

namespace LionFire.Data.Async.Mvvm;

// Without TCollection
public class LazilyGetsCollectionVM<TValue, TValueVM>
    : LazilyGetsCollectionVM<TValue, TValueVM, IEnumerable<TValue>>
{
    #region Lifecycle

    public LazilyGetsCollectionVM(IViewModelProvider viewModelProvider) : base(viewModelProvider)
    {
    }

    #endregion
}

public class LazilyGetsCollectionVM<TValue, TValueVM, TCollection>
    : LazilyGetsVM<TCollection>
    , IResolvesCollectionVM<TValue, TValueVM, TCollection>
    where TCollection : IEnumerable<TValue>
{
    #region Dependencies

    public IViewModelProvider ViewModelProvider { get; }

    #endregion

    #region Lifecycle

    public LazilyGetsCollectionVM(IViewModelProvider viewModelProvider)
    {
        ViewModelProvider = viewModelProvider;
    }

    #endregion
}