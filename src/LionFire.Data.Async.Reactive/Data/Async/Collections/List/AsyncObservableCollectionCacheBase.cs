﻿//using DynamicData;
//using Microsoft.Extensions.GetOptions;

//namespace LionFire.Data.Collections;

//public abstract partial class AsyncObservableCollectionCacheBase<TItem, TCollection> : AsyncObservableCollectionCacheBaseBase<TItem, TCollection>
//    where TCollection : class, IObservableCollection<TItem>, new()
//{
//    #region Lifecycle

//    public AsyncObservableCollectionCacheBase() { }

//    public AsyncObservableCollectionCacheBase(TCollection? collection = null, AsyncObservableCollectionOptions? options = null) : base(collection, options)
//    {
//    }

//    #endregion

//    protected SourceList<TItem> SourceList = new();
//}
