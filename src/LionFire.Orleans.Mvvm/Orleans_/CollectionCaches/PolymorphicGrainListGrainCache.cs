
//using LionFire.Orleans_.Collections;
//using Orleans;
//using LionFire.Collections;
//
//using LionFire.Mvvm;
//using System.ComponentModel;
//using System.Collections;
//using Microsoft.Extensions.DependencyInjection;
//using Newtonsoft.Json.Linq;
//using Orleans.Streams;
//using LionFire.Orleans_.Streams;
//using LionFire.Structures;
////using Orleans.BroadcastChannels;

//namespace LionFire.Orleans_.Mvvm;

///// <summary>
///// Adapter to ListGrainCache that unwraps IPolymorphicListGrainItem&lt;TGrain&gt;
///// </summary>
///// <typeparam name="TInfo"></typeparam>
///// <typeparam name="IPolymorphicGrainListGrain<TInfo>"></typeparam>
//public class PolymorphicGrainListGrainCache<TInfo> : AsyncObservableDictionaryCache<string, TInfo>
//    , IOnBroadcastChannelSubscribed

//    , System.IAsyncObservable<NotifyCollectionChangedEventArgs<TInfo>>
//    , LionFire.Reactive.IAsyncObservableForSyncObservers<NotifyCollectionChangedEventArgs<TInfo>>

//    //, INotifyPropertyChanging
//    //, LionFire.Mvvm.IAsyncCollectionCache<TInfo>
//    //where IPolymorphicGrainListGrain<TInfo> : ICreatingAsyncDictionary<string, IPolymorphicGrainListGrainItem<TInfo>>
//    where TInfo : IGrain
//{

//    #region Relationships

//    public IPolymorphicGrainListGrain<TInfo> Source { get; }

//    #endregion

//    #region Dependencies

//    public IClusterClient ClusterClient { get; }

//    #endregion

//    #region Lifecycle

//    public PolymorphicGrainListGrainCache(
//        //IServiceProvider serviceProvider, 
//        IPolymorphicGrainListGrain<TInfo> source, IClusterClient clusterClient)
//        : base(g => g.GetPrimaryKeyString())
//    {
//        Source = source; // ActivatorUtilities.CreateInstance<PolymorphicGrainListG<TInfo>>(serviceProvider, source);

//        //ListGrainCache = new(source, clusterClient);
//        ClusterClient = clusterClient;
//    }

//    #endregion

//    #region IEnumerable

//    public override bool CanRetrieve => true;
//    protected override async Task<IEnumerable<TInfo>> RetrieveValues(CancellationToken cancellationToken = default)
//        => (await Source.GetEnumerableAsync().ConfigureAwait(false));

//    #endregion


//    #region DictionaryCache

//    public override bool IsReadOnly => false;

//    #endregion

//    #region Create

//    public override bool CanCreate => true;

//    public override async Task<TInfo> Create(Type TNewItem, params object[]? constructorParameters)
//    {
//        if (!CanCreate) { throw new NotSupportedException($"{nameof(CanCreate)}"); }
//        if (!TNewItem.IsAssignableTo(typeof(TInfo))) { throw new ArgumentException($"TNewItem must be assignable to {typeof(TInfo).FullName}"); }
//        if (constructorParameters != null && constructorParameters.Length != 0) { throw new ArgumentException($"{nameof(constructorParameters)} not supported"); }
//        var result = await Source.Create(TNewItem);
//        return result;
//    }

//    #endregion

//    #region Remove

//    public override Task<bool> Remove(TInfo item) => Source.Remove(item);

//    #endregion


//    #region Notifications Stream

//    public IAsyncStream<NotifyCollectionChangedEventArgs<TInfo>> CollectionChangedStream
//    {
//        get
//        {
//            var s = ClusterClient.GetStreamProvider("ChangeNotifications").GetStream<NotifyCollectionChangedEventArgs<TInfo>>(Source.GetPrimaryKeyString(), "CollectionChanged");
//            return s;
//        }
//    }

//    #region LionFire AsyncObserver

//    //OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<TInfo>>? subscriptionAdapter;

//    public ValueTask<IAsyncDisposable> Subscribe(System.IAsyncObserver<NotifyCollectionChangedEventArgs<TInfo>> observer)
//    {
//        //if (subscriptionAdapter == null)
//        //{
//        //    subscriptionAdapter = new OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<TInfo>>(this);
//        //}
//        //return await subscriptionAdapter.Subscribe(observer);

//        return CollectionChangedStream.Subscribe(observer);
//    }
//    public Task<IAsyncDisposable> Subscribe(IObserver<NotifyCollectionChangedEventArgs<TInfo>> observer)
//    {
//        //if (subscriptionAdapter == null)
//        //{
//        //    subscriptionAdapter = new OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<TInfo>>(this);
//        //}
//        //return await subscriptionAdapter.Subscribe(observer);

//        return CollectionChangedStream.Subscribe(observer);
//    }


//    #endregion

//    #endregion



//}
