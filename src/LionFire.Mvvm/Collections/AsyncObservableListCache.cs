﻿using ObservableCollections;

namespace LionFire.Mvvm;


// ENH // ideas specific to lists: sorting
//public class AsyncObservableListOptions<T> : AsyncObservableCollectionOptions
//{
//    public bool SortFromSource { get; set; }

//    public IComparer<T>? Comparer { get; set; }
//}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Not supported yet: ability to control sorting or reordering
/// </remarks>
public class AsyncObservableListCache<T> : AsyncObservableCollectionCacheBase<T, ObservableList<T>>, IAsyncCollectionCache<T>
{
    public AsyncObservableListCache() { }
    public AsyncObservableListCache(IObservableCollection<T>? collection = null, AsyncObservableCollectionOptions? options = null) : base(collection, options) { }

}
