using ObservableCollections;

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
public class AsyncObservableListCache<TItem> : AsyncObservableCollectionCacheBase<TItem, ObservableList<TItem>>
{
    public AsyncObservableListCache() { }
    public AsyncObservableListCache(IObservableCollection<TItem>? collection = null, AsyncObservableCollectionOptions? options = null) : base(collection, options) { }

}
