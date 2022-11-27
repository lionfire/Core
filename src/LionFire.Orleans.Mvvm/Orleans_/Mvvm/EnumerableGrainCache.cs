using LionFire.Mvvm;
using LionFire.Orleans_.Collections;
using LionFire.Structures;
using Newtonsoft.Json.Linq;
using ObservableCollections;
using Orleans;
using Orleans.Streams;
using static LionFire.Reflection.GetMethodEx;

namespace LionFire.Orleans_.Mvvm;

    /// <summary>
    /// Async Cache for IListGrain
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class EnumerableGrainCache<TItem, TCollection> : AsyncObservableListCache<TItem>
    where TCollection : IEnumerableAsync<TItem>
    where TItem : class
// Note: Base class is a list.  Would be cleaner if it was AsyncEnumerableListCache, but probably unneeded
{
    public TCollection Source { get; }
    //public IGrainFactory GrainFactory => ClusterClient;
    //public IClusterClient ClusterClient { get; }

    public EnumerableGrainCache(TCollection source, IObservableCollection<TItem>? collection = null, AsyncObservableCollectionOptions? options = null) : base(collection, options)
    {
        Source = source;
        //ClusterClient = clusterClient;
    }

    public override bool IsReadOnly => true;

    //public virtual string GetKey(TItem item)
    //{
    //    if (item is IKeyed<string> k)
    //    {
    //        return k.Key;
    //    }
    //    else
    //    {
    //        throw new NotSupportedException($"Since {nameof(TItem)} does not implement IKeyed<string>, GetKey must be overridden with a valid implementation.");
    //    }
    //}

    #region Retrieve

    public override bool CanRetrieve => true;

    protected override async Task<IEnumerable<TItem>> RetrieveImpl(CancellationToken cancellationToken = default)
        => (await Source.GetEnumerableAsync())
        //.Select(i => (TItem)GrainFactory.GetGrain(i.Type, i.Id))
        ;

    #endregion

    protected override Task<bool> Subscribe()
    {
        throw new NotImplementedException();
        //var streamProvider = ClusterClient.GetStreamProvider("ChangeNotifications"); // HARDCODE TODO
        //var stream = streamProvider.GetStream<string>(Source.GetPrimaryKeyString(), "Collection"); // HARDCODE TODO
        //var subscriptionHandle = await stream.SubscribeAsync(async (message, token) => Console.WriteLine(message));

        //return true; // TODO REVIEW
    }

#if TODO
    /// <summary>
    /// Can call Subscribe() and Unsubscribe().  Do not call these directly; set Sync instead.
    /// </summary>
    public override bool CanSubscribe => true;

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if actually subscribed to something during this invocation, false if already subscribed (throw on fail)</returns>
    /// <exception cref="NotSupportedException"></exception>
    protected override Task<bool> Subscribe() => throw new NotSupportedException();
    protected override Task Unsubscribe() => throw new NotSupportedException();
#endif

}
