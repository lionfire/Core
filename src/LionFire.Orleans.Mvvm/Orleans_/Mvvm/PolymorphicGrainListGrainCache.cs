
using LionFire.Orleans_.Collections;
using Orleans;
using LionFire.Collections;
using LionFire.Orleans_.Collections.PolymorphicGrainListGrain;
using LionFire.Mvvm;
using System.ComponentModel;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Orleans.Streams;
using LionFire.Orleans_.Streams;
using LionFire.Structures;

namespace LionFire.Orleans_.Mvvm;

/// <summary>
/// Adapter to ListGrainCache that unwraps IPolymorphicListGrainItem&lt;TGrain&gt;
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="IPolymorphicGrainListGrain<TItem>"></typeparam>
public class PolymorphicGrainListGrainCache<TItem> : AsyncObservableListCache<TItem>

    , LionFire.Reactive.IAsyncObservable<NotifyCollectionChangedEventArgs<TItem>>
    , LionFire.Reactive.IAsyncObservableForSyncObservers<NotifyCollectionChangedEventArgs<TItem>>

    //, INotifyPropertyChanging
    //, LionFire.Mvvm.IAsyncCollectionCache<TItem>
    //where IPolymorphicGrainListGrain<TItem> : ICreatingAsyncDictionary<string, IPolymorphicGrainListGrainItem<TItem>>
    where TItem : IGrain
{

    #region Relationships

    public IPolymorphicGrainListGrain<TItem> Source { get; }

    #endregion

    #region Dependencies

    public IClusterClient ClusterClient { get; }

    #endregion

    #region Lifecycle

    public PolymorphicGrainListGrainCache(IServiceProvider serviceProvider, IPolymorphicGrainListGrain<TItem> source, IClusterClient clusterClient)
    {
        Source = source; // ActivatorUtilities.CreateInstance<PolymorphicGrainListGrain<TItem>>(serviceProvider, source);

        //ListGrainCache = new(source, clusterClient);
        ClusterClient = clusterClient;
    }

    #endregion


    #region IEnumerable

    public override bool CanRetrieve => true;
    protected override Task<IEnumerable<TItem>> RetrieveImpl(CancellationToken cancellationToken = default)
        => Source.GetEnumerableAsync();

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


    #region Notifications Stream

    public IAsyncStream<NotifyCollectionChangedEventArgs<TItem>> CollectionChangedStream
    {
        get
        {
            var s = ClusterClient.GetStreamProvider("ChangeNotifications").GetStream<NotifyCollectionChangedEventArgs<TItem>>(Source.GetPrimaryKeyString(), "CollectionChanged");
            return s;
        }
    }

    #region LionFire AsyncObserver

    //OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<TItem>>? subscriptionAdapter;

    public Task<IAsyncDisposable> SubscribeAsync(Reactive.IAsyncObserver<NotifyCollectionChangedEventArgs<TItem>> observer)
    {
        //if (subscriptionAdapter == null)
        //{
        //    subscriptionAdapter = new OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<TItem>>(this);
        //}
        //return await subscriptionAdapter.SubscribeAsync(observer);

        return CollectionChangedStream.SubscribeAsync(observer);
    }
    public Task<IAsyncDisposable> SubscribeAsync(IObserver<NotifyCollectionChangedEventArgs<TItem>> observer)
    {
        //if (subscriptionAdapter == null)
        //{
        //    subscriptionAdapter = new OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<TItem>>(this);
        //}
        //return await subscriptionAdapter.SubscribeAsync(observer);

        return CollectionChangedStream.SubscribeAsync(observer);
    }


    #endregion

    #endregion



}
