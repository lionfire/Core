#if UNUSED // Maybe useful someday for non-polymorphic lists
using LionFire.Orleans_.Collections;
//using ObservableCollections;
using Orleans;
using static LionFire.Reflection.GetMethodEx;
using System.Collections.ObjectModel;
using LionFire.Structures;
using System.ComponentModel;
using Orleans.Streams;
using System.Collections.Specialized;
using LionFire.Collections;
using LionFire.Reflection;
using System.Reflection.Metadata.Ecma335;
using LionFire.Orleans_.Mvvm;
using LionFire.Reactive;
using System.Runtime.CompilerServices;
using LionFire.Orleans_.Streams;
using Newtonsoft.Json.Linq;

namespace LionFire.Orleans_.Mvvm;

public class NotifyingListGrainCache<TItem, TCollection> : ListGrainCache<TItem, TCollection>
    , LionFire.Reactive.IAsyncObservable<NotifyCollectionChangedEventArgs<string>>
    , LionFire.Reactive.IAsyncObservableForSyncObservers<NotifyCollectionChangedEventArgs<string>>
    where TCollection : IListAsync<TItem>, IAsyncCreating<TItem>, IGrain
    where TItem : class, IGrain
{
    public IClusterClient ClusterClient { get; }

    #region Lifecycle

    public NotifyingListGrainCache(TCollection source, IClusterClient clusterClient) : base(source)
    {
        ClusterClient = clusterClient;
    }

    #endregion

    #region Notifications Stream

    public IAsyncStream<NotifyCollectionChangedEventArgs<string>> CollectionChangedStream
    {
        get
        {
            var s = ClusterClient.GetStreamProvider("ChangeNotifications").GetStream<NotifyCollectionChangedEventArgs<string>>(Guid.Parse(Source.GetPrimaryKeyString()), "CollectionChanged");
            return s;
        }
    }    

    #region LionFire AsyncObserver

    //OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<string>>? subscriptionAdapter;

    public Task<IAsyncDisposable> SubscribeAsync(Reactive.IAsyncObserver<NotifyCollectionChangedEventArgs<string>> observer)
    {
        //if (subscriptionAdapter == null)
        //{
        //    subscriptionAdapter = new OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<string>>(this);
        //}
        //return await subscriptionAdapter.SubscribeAsync(observer);

        return CollectionChangedStream.SubscribeAsync(observer);
    }
    public Task<IAsyncDisposable> SubscribeAsync(IObserver<NotifyCollectionChangedEventArgs<string>> observer)
    {
        //if (subscriptionAdapter == null)
        //{
        //    subscriptionAdapter = new OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<string>>(this);
        //}
        //return await subscriptionAdapter.SubscribeAsync(observer);

        return CollectionChangedStream.SubscribeAsync(observer);
    }


    #endregion

    #endregion
    
}


/// <summary>
/// Writable Async Cache for an IListGrain
/// </summary>
/// <typeparam name="T"></typeparam>

public class ListGrainCache<TItem, TCollection> : EnumerableGrainCache<TItem, TCollection>
    //Orleans.Streams.IAsyncObservable<NotifyCollectionChangedEventArgs<string>>,
    where TCollection : IListAsync<TItem>, IAsyncCreating<TItem>
    where TItem : class
{
    #region Lifecycle

    public ListGrainCache(TCollection source) : base(source)
    {
    }

    #endregion

    #region Collection

    public override bool IsReadOnly => false;

    #endregion

    #region Create

    public override bool CanCreate => true;

    public override async Task<TItem> Create(Type type, params object[]? constructorParameters)
    {
        if (!CanCreate) { throw new NotSupportedException($"{nameof(CanCreate)}"); }
        if (!type.IsAssignableTo(typeof(TItem))) { throw new ArgumentException($"type must be assignable to {typeof(TItem).FullName}"); }
        if (constructorParameters != null && constructorParameters.Length != 0) { throw new ArgumentException($"{nameof(constructorParameters)} not supported"); }
        var result = await Source.Create(type);
        return result;
    }

    #endregion

    #region Add

    public override Task Add(TItem item)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Remove

    public override async Task<bool> Remove(TItem item)
    {
        if (collection == null) { throw new InvalidOperationException($"Cannot invoke while {nameof(Collection)} is null"); }

        var removedFromInternal = collection.Remove(item);

        if (Source == null)
        {
            return removedFromInternal;
        }
        else
        {
            bool removedFromSource = false;
            try
            {
                removedFromSource = await Source.Remove(item);
            }
            catch
            {
                removedFromSource = false;
                try
                {
                    collection.Add(item);
                }
                catch { } // EMPTYCATCH
                throw;
            }
            return removedFromSource;
        }
    }

    #endregion
    
}

#endif