using DynamicData;
using LionFire.Structures.Keys;
using System.Reactive.Subjects;
using System.Reactive;

namespace LionFire.Data.Async.Collections;


// ENH // ideas specific to lists: sorting
//public class AsyncObservableListOptions<TValue> : AsyncObservableCollectionOptions
//{
//    public bool SortFromSource { get; set; }

//    public IComparer<TValue>? Comparer { get; set; }
//}

///// <summary>
///// 
///// </summary>
///// <typeparam name="TValue"></typeparam>
///// <remarks>
///// Not supported yet: ability to control sorting or reordering
///// </remarks>
//public class AsyncObservableListCache<TItem> : AsyncObservableCollectionCacheBase<TItem, ObservableList<TItem>>
//{
//    public AsyncObservableListCache() { }
//    public AsyncObservableListCache(ObservableList<TItem>? collection = null, AsyncObservableCollectionOptions? options = null) : base(collection, options) { }

//}

// Future: ReactiveUI?
//public class AsyncReactiveListCache<TValue> : AsyncObservableCollectionCacheBase<TValue, ReactiveList<TValue>>
//{
//    public AsyncReactiveListCache() { }
//    public AsyncReactiveListCache(ReactiveList<TValue>? collection = null, AsyncObservableCollectionOptions? options = null) : base(collection, options) { }

//}

public abstract class AsyncReadOnlyListCache<TValue> 
    : AsyncDynamicDataCollectionCache<TValue>
    , System.IAsyncObserver<ChangeSet<TValue>>
{

    #region Lifecycle

    public AsyncReadOnlyListCache() : this(null) { }
    public AsyncReadOnlyListCache(SourceList<TValue>? sourceList)
    {
        this.SourceList = sourceList ?? new SourceList<TValue>();
    }

    #endregion

    #region State

    protected SourceList<TValue> SourceList { get; }
    //public override DynamicData.IObservableList<TValue> List => SourceList.AsObservableList();

    #endregion

    #region IAsyncObserver

    public ValueTask OnNextAsync(ChangeSet<TValue> value)
    {
        SourceList.Edit(u => u.Clone(value));
        return ValueTask.CompletedTask;
    }

    public IObservable<Exception> AsyncObserverErrors => asyncObserverErrors.Value;
    private Lazy<Subject<Exception>> asyncObserverErrors = new();

    public ValueTask OnErrorAsync(Exception error)
    {
        asyncObserverErrors.Value.OnNext(error);
        return ValueTask.CompletedTask;
    }

    public IObservable<Unit> AsyncObserverCompleted => asyncObserverCompleted.Value;
    private Lazy<Subject<Unit>> asyncObserverCompleted = new();

    public ValueTask OnCompletedAsync()
    {
        asyncObserverCompleted.Value.OnNext(Unit.Default);
        return ValueTask.CompletedTask;
    }

    #endregion
}
