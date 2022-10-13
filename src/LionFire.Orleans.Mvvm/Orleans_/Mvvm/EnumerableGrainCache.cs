using LionFire.Mvvm;
using LionFire.Orleans_.Collections;
using ObservableCollections;
using Orleans;
using Orleans.Streams;
using static LionFire.Reflection.GetMethodEx;

namespace LionFire.Orleans_.Mvvm;

/// <summary>
/// Async Cache for IListGrain
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class EnumerableGrainCache<TCollection, TItem> : AsyncObservableListCache<GrainListItem<TItem>>
    where TCollection : IEnumerableGrain<GrainListItem<TItem>>
    where TItem : class, IGrain
{
    public TCollection Source { get; }
    public IGrainFactory GrainFactory { get; }
    public IClusterClient ClusterClient { get; }

    public EnumerableGrainCache(TCollection source, IGrainFactory grainFactory, IClusterClient clusterClient, IObservableCollection<GrainListItem<TItem>>? collection = null, AsyncObservableCollectionOptions? options = null) : base(collection, options)
    {
        Source = source;
        GrainFactory = grainFactory;
        ClusterClient = clusterClient;
    }

    public override bool IsReadOnly => true;

    #region Retrieve

    public override bool CanRetrieve => true;
    
    protected override async Task<IEnumerable<GrainListItem<TItem>>> RetrieveImpl(CancellationToken cancellationToken = default)
        => (await Source.Items())
                //.Select(i => (TItem)GrainFactory.GetGrain(i.Type, i.Id))
        ;

    #endregion

    protected override Task<bool> Subscribe()
    {
        throw new NotImplementedException();
        //var streamProvider = ClusterClient.GetStreamProvider("CollectionStreamProvider"); // HARDCODE TODO
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
