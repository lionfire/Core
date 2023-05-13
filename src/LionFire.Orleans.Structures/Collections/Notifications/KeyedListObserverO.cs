using DynamicData;
using LionFire.ExtensionMethods.Cloning;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LionFire.Orleans_.Collections;

public class KeyedListObserverO<TKey, TItem>
    : IGrainObserverO<ChangeSet<TItem, TKey>>
    , IObservable<ChangeSet<TItem, TKey>>
    where TKey : notnull
{
    #region (Private) fields

    //CompositeAsyncDisposable disposables = new CompositeAsyncDisposable();
    List<IAsyncDisposable> asyncDisposables = new List<IAsyncDisposable>();
    protected IObservable<ChangeSet<TItem>> observable;

    #endregion

    #region Lifecycle

    public KeyedListObserverO(IChangeSetObservableG<TItem, TKey> keyedCollectionGrain)
    {
        observable = Observable.FromAsync<ChangeSet<TItem>>(async () =>
            {
                asyncDisposables.Add(await keyedCollectionGrain.SubscribeAsync(this));
                //await disposables.AddAsync(await collectionGrain.Subscribe(this));
            })
        .Publish()
        .RefCount();
    }

    ValueTask DisposeAsync()
    {
        asyncDisposables.
    }

    #endregion

    #region State

    protected Subject<ChangeSet<TItem>> subject = new();

    #endregion

    #region IObservable


    public IDisposable Subscribe(IObserver<TChangeSet> observer)
        => ((IObservable<ChangeSet<TItem>>)subject).Subscribe(observer);

    #endregion

    public void BindTo(SourceCache<TItem, TKey> observableCache)
    {
        this.observable.Subscribe(cs =>
        {
            observableCache.Edit(u => u.Clone(cs));
        });
    }


    #region IAsyncObserver

    //public ValueTask OnCompleted()
    //{
    //    ((IObserver<ChangeSet<TNotificationItem>>)subject).OnCompleted();
    //    return ValueTask.CompletedTask;
    //}

    //public ValueTask OnError(Exception error)
    //{
    //    ((IObserver<ChangeSet<TNotificationItem>>)subject).OnError(error);
    //    return ValueTask.CompletedTask;
    //}

    //public ValueTask OnNext(ChangeSet<TNotificationItem> value)
    //{
    //    Logger.LogInformation("OnNext: " + value);
    //    ((IObserver<ChangeSet<TNotificationItem>>)subject).OnNext(value);
    //    return ValueTask.CompletedTask;
    //}

    #endregion



    #region System.IAsyncObserver<ChangeSet<TItem>> pass-thru to this.subject

    public ValueTask OnNextAsync(ChangeSet<TItem> value)
    {
        subject.OnNext(value);
        return ValueTask.CompletedTask;
    }

    public ValueTask OnErrorAsync(Exception error)
    {
        subject.OnError(error);
        return ValueTask.CompletedTask;
    }

    public ValueTask OnCompletedAsync()
    {
        subject.OnCompleted();
        return ValueTask.CompletedTask;
    }

    #endregion

}

#if ENH // Optimize: only send GrainId over the network.  Missing dependency: Convert ChangeSet<TItem, TKey> to ChangeSet<TItem>

/// <summary>
/// Listens to TNotificationItem changes, and publishes ChangeSet<TNotificationItem, TKey> changes
/// 
/// Created for this scenario:
///  - GrainIds sent over the network
///  - TOutputItem is Grain types
///  - TOutputKey is the same GrainId that was sent over the network
/// </summary>
/// <typeparam name="TOutputItem"></typeparam>
/// <typeparam name="TNotificationItem"></typeparam>
public class TransformingKeySelectingListObserverO<TNotificationItem, TOutputKey, TOutputItem>
    : KeyedListObserverBaseO<TOutputKey, TOutputItem, ChangeSet<TNotificationItem>>
    , IChangeSetGrainObserver<TNotificationItem>
    , IObservable<ChangeSet<TNotificationItem>>
    where TOutputKey : notnull
{
    private readonly Func<TNotificationItem, TOutputItem> KeySelector;

    #region Lifecycle

    public TransformingKeySelectingListObserverO(Func<TNotificationItem, (TOutputKey key, TOutputItem item)> keySelector, IChangeSetObservableG<TNotificationItem> collectionGrain)
    {
        KeySelector = keySelector;

        observable = Observable.FromAsync<ChangeSet<TNotificationItem>>(async () =>
        {
            asyncDisposables.Add(await collectionGrain.SubscribeAsync(this));
            //await disposables.AddAsync(await collectionGrain.Subscribe(this));
        })
            .Publish()
            .RefCount()
            ;
    }

    #endregion
}

public abstract class KeyedListObserverBaseO<TKey, TItem, TChangeSet>
    : IObservable<ChangeSet<TItem, TKey>>
    where TKey : notnull
{
    #region (Private) fields


    #endregion

    
}
#endif

///// <summary>
///// Not Implemented
///// </summary>
///// <typeparam name="TItem"></typeparam>
//public class KeyedListObserverO<TItem>
//    : IGrainObserverO<TKey>
//    , IObservable<ChangeSet<TItem>>
//{
//    public KeyedListObserverO()
//    {
//        throw new NotImplementedException();
//    }

//    #region IObservable

//    IObservable<ChangeSet<TItem>> observable;

//    public IDisposable Subscribe(IObserver<ChangeSet<TItem>> observer)
//        => ((IObservable<ChangeSet<TItem>>)subject).Subscribe(observer);

//    #endregion
//}
