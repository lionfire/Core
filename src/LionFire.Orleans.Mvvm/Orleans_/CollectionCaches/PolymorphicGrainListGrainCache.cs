
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
///// <typeparam name="TValue"></typeparam>
///// <typeparam name="IPolymorphicGrainListGrain<TValue>"></typeparam>
//public class PolymorphicGrainListGrainCache<TValue> : AsyncObservableDictionaryCache<string, TValue>
//    , IOnBroadcastChannelSubscribed

//    , System.IAsyncObservable<NotifyCollectionChangedEventArgs<TValue>>
//    , LionFire.Reactive.IAsyncObservableForSyncObservers<NotifyCollectionChangedEventArgs<TValue>>

//    //, INotifyPropertyChanging
//    //, LionFire.Mvvm.IAsyncCollectionCache<TValue>
//    //where IPolymorphicGrainListGrain<TValue> : ICreatingAsyncDictionary<string, IPolymorphicGrainListGrainItem<TValue>>
//    where TValue : IGrain
//{

//    #region Relationships

//    public IPolymorphicGrainListGrain<TValue> Source { get; }

//    #endregion

//    #region Dependencies

//    public IClusterClient ClusterClient { get; }

//    #endregion

//    #region Lifecycle

//    public PolymorphicGrainListGrainCache(
//        //IServiceProvider serviceProvider, 
//        IPolymorphicGrainListGrain<TValue> source, IClusterClient clusterClient)
//        : base(g => g.GetPrimaryKeyString())
//    {
//        Source = source; // ActivatorUtilities.CreateInstance<PolymorphicGrainListG<TValue>>(serviceProvider, source);

//        //ListGrainCache = new(source, clusterClient);
//        ClusterClient = clusterClient;
//    }

//    #endregion

//    #region IEnumerable

//    public override bool CanRetrieve => true;
//    protected override async Task<IEnumerable<TValue>> RetrieveValues(CancellationToken cancellationToken = default)
//        => (await Source.GetEnumerableAsync().ConfigureAwait(false));

//    #endregion


//    #region DictionaryCache

//    public override bool IsReadOnly => false;

//    #endregion

//    #region Create

//    public override bool CanCreate => true;

//    public override async Task<TValue> Create(Type TNewItem, params object[]? constructorParameters)
//    {
//        if (!CanCreate) { throw new NotSupportedException($"{nameof(CanCreate)}"); }
//        if (!TNewItem.IsAssignableTo(typeof(TValue))) { throw new ArgumentException($"TNewItem must be assignable to {typeof(TValue).FullName}"); }
//        if (constructorParameters != null && constructorParameters.Length != 0) { throw new ArgumentException($"{nameof(constructorParameters)} not supported"); }
//        var result = await Source.Create(TNewItem);
//        return result;
//    }

//    #endregion

//    #region Remove

//    public override Task<bool> Remove(TValue item) => Source.Remove(item);

//    #endregion


//    #region Notifications Stream

//    public IAsyncStream<NotifyCollectionChangedEventArgs<TValue>> CollectionChangedStream
//    {
//        get
//        {
//            var s = ClusterClient.GetStreamProvider("ChangeNotifications").GetStream<NotifyCollectionChangedEventArgs<TValue>>(Source.GetPrimaryKeyString(), "CollectionChanged");
//            return s;
//        }
//    }

//    #region LionFire AsyncObserver

//    //OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<TValue>>? subscriptionAdapter;

//    public ValueTask<IAsyncDisposable> Subscribe(System.IAsyncObserver<NotifyCollectionChangedEventArgs<TValue>> observer)
//    {
//        //if (subscriptionAdapter == null)
//        //{
//        //    subscriptionAdapter = new OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<TValue>>(this);
//        //}
//        //return await subscriptionAdapter.Subscribe(observer);

//        return CollectionChangedStream.Subscribe(observer);
//    }
//    public Task<IAsyncDisposable> Subscribe(IObserver<NotifyCollectionChangedEventArgs<TValue>> observer)
//    {
//        //if (subscriptionAdapter == null)
//        //{
//        //    subscriptionAdapter = new OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<TValue>>(this);
//        //}
//        //return await subscriptionAdapter.Subscribe(observer);

//        return CollectionChangedStream.Subscribe(observer);
//    }


//    #endregion

//    #endregion



//}
