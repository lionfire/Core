#define RedisCollections
using DynamicData;
using LionFire.Orleans_;
using LionFire.Orleans_.Collections;
using LionFire.Orleans_.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Persistence;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text;

namespace LionFire.Orleans_.Workers.Grains;

// ENH ideas:
// - persisted:
//   - Info
//   - BecameInactiveOn
// - unpersisted state

// ENH - persisted
//public class GrainObserverRegistrar<TGrainObserver, TPersistedState> : GrainObserverRegistrar<TGrainObserver>
//{
//}

public class GrainObserverRegistrar<TGrainObserver>
    : InMemoryGrainObserverRegistrar<TGrainObserver>
    , IGrainObserverRegistrar<TGrainObserver>
    where TGrainObserver : notnull, IAddressable, IGrainObserver, IGrainWithStringKey
{
    IGrainFactory grainFactory;

    #region Lifecycle

    public GrainObserverRegistrar(IServiceProvider serviceProvider, ILogger logger
#if !RedisCollections
      ,  IPersistentState<List<string>> persistentState
#else 
    , string redisStoreName
#endif
        ) : base(serviceProvider, logger)
    {
#if !RedisCollections
        this.persistentState = persistentState;
#endif
        items.Connect().Subscribe(OnItemsChanged);

#if RedisCollections
        grainFactory = ServiceProvider.GetRequiredService<IGrainFactory>();
        IOptions<ClusterOptions> clusterOptions = serviceProvider.GetRequiredService<IOptions<ClusterOptions>>();
        _options = serviceProvider.GetRequiredService<IOptionsMonitor<RedisStorageOptions>>().Get(redisStoreName);
        var _serviceId = clusterOptions.Value.ServiceId;
        _keyPrefix = Encoding.UTF8.GetBytes($"{_serviceId}/state/");
        _getKeyFunc = this._options.GetStorageKey ?? DefaultGetStorageKey;

        redis = ConnectionMultiplexer.Connect(_options.ConfigurationOptions ?? throw new ArgumentNullException("Failed to get redis ConfigurationOptions from named Orleans redis source: " + redisStoreName));
#endif

    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var existing = await DB.HashGetAllAsync(RedisKey);

        if (existing.Length == 0)
        {
            HashEntry[] emptyHash = Array.Empty<HashEntry>();
            await DB.HashSetAsync(RedisKey, emptyHash);
        }
        else
        {
#if false
            foreach (var entry in existing)
            {
                var key = entry.Name.ToString();
                var value = entry.Value.ToString();
                TGrainObserver observer;
                try
                {
#warning NEXT: Deserialize GrainObserver, but this doesn't make sense, so move this to a general purpose redis collection class
                    //observer = grainFactory.CreateObjectReference<TGrainObserver>(key); // FIXME - are grain observers available from the global membership directory?
                }
                catch
                {
                    await DB.HashDeleteAsync(this.RedisKey, key);
                    continue;
                }
                observerManager.Subscribe(observer, observer);
            }
#endif
        }

        await base.OnActivateAsync(cancellationToken);
    }

#endregion

    #region State

#if RedisCollections
    RedisGrainStorage redisGrainStorage;
    ConnectionMultiplexer redis;
    IDatabase DB => redis.GetDatabase();
#else
    IPersistentState<List<string>> persistentState;
#endif

    List<IChangeSet<IObserverEntry<TGrainObserver>, IAddressable>> pendingWrites = new();

    #endregion

    #region Serialization

    //public ISimServerWorkerO FromSerializableValue(string key) => grainFactory.GetGrain<ISimServerWorkerO>(key);
    public string GetSerializableValue(TGrainObserver o) => o.GetGrainId().Key.ToString()!;

    // Based on official Orleans implementation
    private readonly RedisStorageOptions _options;
    private readonly RedisKey _keyPrefix;
    private readonly Func<string, GrainId, RedisKey> _getKeyFunc;
    private string GrainType => this.GetType().Name.ToLowerInvariant(); // TODO - how does Orleans do this?
    private RedisKey DefaultGetStorageKey(string grainType, GrainId grainId)
    {
        var grainIdTypeBytes = IdSpan.UnsafeGetArray(grainId.Type.Value);
        var grainIdKeyBytes = IdSpan.UnsafeGetArray(grainId.Key);
        var grainTypeLength = Encoding.UTF8.GetByteCount(grainType);
        var suffix = new byte[grainIdTypeBytes!.Length + 1 + grainIdKeyBytes!.Length + 1 + grainTypeLength];
        var index = 0;

        grainIdTypeBytes.CopyTo(suffix, 0);
        index += grainIdTypeBytes.Length;

        suffix[index++] = (byte)'/';

        grainIdKeyBytes.CopyTo(suffix, index);
        index += grainIdKeyBytes.Length;

        suffix[index++] = (byte)'/';

        var bytesWritten = Encoding.UTF8.GetBytes(grainType, suffix.AsSpan(index));

        Debug.Assert(bytesWritten == grainTypeLength);
        Debug.Assert(index + bytesWritten == suffix.Length);
        return _keyPrefix.Append(suffix);
    }


    #endregion
    private string RedisKey => _getKeyFunc(GrainType, this.GetGrainId())!;

    #region Handlers

    private async void OnItemsChanged(IChangeSet<IObserverEntry<TGrainObserver>, IAddressable> set)
    {
        pendingWrites.Add(set);
        await OnStateHasChanged();
    }

    protected override async ValueTask OnStateHasChanged()
    {
        await base.OnStateHasChanged();
#if RedisCollections
        var db = DB;
        var key = RedisKey;

#if false // Tips: Batch, efficiency

        Efficiency Tips:
Batch Operations: When adding or removing multiple items, it's more efficient to use batch operations as shown above. This reduces the number of round trips to the Redis server.
Transactions: For operations that need to be atomic, consider using Redis transactions or Lua scripts.Here's a simple example of a transaction:
csharp
var trans = db.CreateTransaction();
        trans.AddCondition(Condition.HashEqual("myhash", "field1", "value1"));
        trans.HashSetAsync("myhash", "field2", "newValue2");
        trans.HashSetAsync("myhash", "field3", "newValue3");

        bool committed = trans.Execute();
        if (committed)
        {
            Console.WriteLine("Transaction committed");
        }
        else
        {
            Console.WriteLine("Transaction not executed because condition failed");
        }
    Pipeline: For operations where the results aren't needed immediately, pipelining can greatly improve performance:
csharp
var tasks = new[]
{
    db.HashSetAsync("myhash", "field4", "value4"),
    db.HashSetAsync("myhash", "field5", "value5")
};

        await Task.WhenAll(tasks);

#endif
        foreach (var set in pendingWrites)
        {
            List<Task> tasks = new();

            if (pendingWrites
                        .SelectMany(pw => pw)
                        .Where(c => c.Reason == ChangeReason.Refresh).Any())
            {
                await Task.WhenAll(tasks);
                tasks.Clear();
                // Alternative to deleting:
                // Using a transaction or Lua script to rename the hash, create a new empty hash with the original name, and then delete the renamed hash. Here's how you could do it with a transaction:
                await db.KeyDeleteAsync(key);
            }

            bool batchWithinChangeSet = true;
            if (batchWithinChangeSet)
            {
                // REVIEW TODO - batching these may lead to data loss in a more generalized collection scenario with rapid turnover
                tasks.Add(db.HashSetAsync(key,
                    set
                        .Where(c => c.Reason == ChangeReason.Add)
                        .Select(c => new HashEntry(
                            c.Key.GetPrimaryKeyString(), c.Key.GetPrimaryKeyString())).ToArray()
                    ));

                tasks.Add(db.HashDeleteAsync(key,
                    set
                       .Where(c => c.Reason == ChangeReason.Remove)
                       .Select(c => (RedisValue)c.Key.GetPrimaryKeyString()).ToArray()
                    ));
            }
            else
            {
                foreach (var change in set)
                {
                    throw new NotImplementedException();
                    //var serializableValue = GetSerializableValue(change.Current.Observer);
                    //if (change.Reason == ChangeReason.Add)
                    //{
                    //    tasks.Add(db.HashSetAsync(key, [
                    //        new(serializableValue, serializableValue),
                    //]));
                    //}
                    //else if (change.Reason == ChangeReason.Remove)
                    //{
                    //    persistentState.State.RemoveKey(serializableValue);
                    //}
                }
            }

            await Task.WhenAll(tasks);
        }
        pendingWrites.Clear();
#else
        persistentState.State = items.Items.Select(i => GetSerializableValue(i.Observer)).ToList();
        await persistentState.WriteStateAsync();
#endif
    }

    #endregion
}


public class InMemoryGrainObserverRegistrar<TGrainObserver>
    : KeyedCollectionGBase<IAddressable, IObserverEntry<TGrainObserver>>
    , IGrainObserverRegistrar<TGrainObserver>
    where TGrainObserver : notnull, IAddressable, IGrainObserver
{
    #region Configuration

    public ValueTask<TimeSpan> RegistrationTimeout() => ValueTask.FromResult(registrationTimeout);
    //public TimeSpan registrationTimeout => TimeSpan.FromMinutes(10); // REVIEW: public with camel case?
    public TimeSpan registrationTimeout => TimeSpan.FromSeconds(20); // REVIEW: public with camel case?

    #endregion

    #region Lifecycle

    static int objectId = 0;
    public int OBJECTID = objectId++;
    public InMemoryGrainObserverRegistrar(IServiceProvider serviceProvider, ILogger logger) : base(serviceProvider, logger, e => e.Observer)
    {
        observerManager = new(registrationTimeout, logger, items);

        observerManager.OnDefunct = async (defunctAddressables) =>
        {
            Logger.LogWarning("{GrainKey} - Defunct observerManager: {DefunctAddresses}", this.GetPrimaryKeyString(), defunctAddressables.Select(o => o.GetPrimaryKeyString()).AggregateOrDefault((x, y) => $"{x}, {y}"));

            //var pairs = defunctAddressables.Select(k =>
            //{
            //    if (!observerManager.Observers.TryGetValue(k, out var v)) throw new InvalidOperationException();
            //    return (k, v);
            //});

            //foreach (var item in defunctAddressables)
            //{
            //    await channel.Writer.WriteAsync(new ChangeSet<TGrainObserver, IAddressable>([new Change<TGrainObserver, IAddressable>(ChangeReason.Remove, item, default)]));
            //}
            //await OnRemoved(defunctAddressables);
        };


        this.RegisterGrainTimer(() =>
        {
            var count = observerManager.Count;
            observerManager.CheckForDefunct();
            if (count != observerManager.Count)
            {
                Logger.LogWarning($"Defunct {typeof(TGrainObserver)} count: {count - observerManager.Count}");
            }
            return Task.CompletedTask;
        }, new GrainTimerCreationOptions(registrationTimeout, registrationTimeout));
        //RegisterTimer(_ =>
        //{
        //    observerManager.CheckForDefunct();
        //    return Task.CompletedTask;
        //}, null!, registrationTimeout, registrationTimeout);

        // OLD
        //CancellationTokenSource = new();

        //Task.Run(async () =>
        //{
        //    PeriodicTimer = new(registrationTimeout);

        //    while (!CancellationTokenSource.IsCancellationRequested)
        //    {
        //        await PeriodicTimer.WaitForNextTickAsync(CancellationTokenSource.Token);
        //        await observerManager.CheckForDefunct();
        //    }
        //});
    }

    // OLD
    //PeriodicTimer PeriodicTimer;
    //CancellationTokenSource CancellationTokenSource;

    #endregion

    #region State

    //protected override IObservableCache<IObserverEntry<TGrainObserver>, IAddressable> ItemsDictionary => observerManager.Observers;

    protected readonly ObserverManagerEx<TGrainObserver> observerManager;
    //public IEnumerable<TGrainObserver> Items => keys.Observers;

    #endregion

    #region (Public) Methods

    public async Task Register(TGrainObserver item)
    {
        var exists = observerManager.Observers.Keys.Contains(item); // TODO: Expiration check? 

        observerManager.Subscribe(item, item);
        //await channel.Writer.WriteAsync(new ChangeSet<TGrainObserver, IAddressable>([new Change<TGrainObserver, IAddressable>(ChangeReason.Add, item, item)]));

        if (!exists)
        {
            Logger.LogInformation("New registration of {type}: {worker}", typeof(TGrainObserver), item);
            await OnRegistered(item);
        }
    }

    public ValueTask Unregister(TGrainObserver item)
    {
        var lookup = observerManager.Observers.Lookup(item);
        observerManager.Unsubscribe(item); // Do it either way
        if (lookup.HasValue)
        {
            //await OnRemoved(item, lookup.Value);
        }
        else
        {
            //await OnRemoved(item, default!);
            Logger.LogWarning($"Unregister: ignoring unknown item '{item}'");
        }
        return ValueTask.CompletedTask;
    }
#if OLD
    private async ValueTask OnRemoved(IAddressable addressable, IObserverEntry<TGrainObserver>? value)
    {
        //await channel.Writer.WriteAsync(new ChangeSet<TGrainObserver, IAddressable>([new Change<TGrainObserver, IAddressable>(ChangeReason.Add, addressable, value.Observer)]));  // OLD for testing
        TGrainObserver o = default!;
        if (value != null) o = value.Observer;

        await OnUnregistered(addressable, o);
        await channel.Writer.WriteAsync(new ChangeSet<TGrainObserver, IAddressable>([new Change<TGrainObserver, IAddressable>(ChangeReason.Remove, addressable, value.Observer)])); // REVIEW: Can the value be default for Remove?

    }
    //private async ValueTask OnRemoved(IEnumerable<IAddressable> keys)
    //{
    //    await channel.Writer.WriteAsync(new ChangeSet<TGrainObserver, IAddressable>(keys.Select(a => new Change<TGrainObserver, IAddressable>(ChangeReason.Remove, a, default!)))); // REVIEW: the !: Can the value be default for Remove?
    //}
#endif

    #endregion

    #region Events

    #region (protected)

    protected virtual Task OnRegistered(TGrainObserver worker) => Task.CompletedTask;
    protected virtual Task OnUnregistered(IAddressable addressable, TGrainObserver worker) => Task.CompletedTask;

    #endregion

    #region (public)

    public ValueTask<IEnumerable<TGrainObserver>> GetAll() => ValueTask.FromResult<IEnumerable<TGrainObserver>>(observerManager.Observers.Items.Select(e => e.Observer).ToList());

    public IAsyncEnumerable<IChangeSet<TGrainObserver, IAddressable>> GetUpdates(CancellationToken cancellationToken = default)
    {
        return observerManager.Observers.Connect()
            .Transform(cs => cs.Observer)
            .ToAsyncEnumerable();
    }

    // OLD: Misses out on Defunct removals
    //[Obsolete]
    //public async IAsyncEnumerable<IChangeSet<TGrainObserver, IAddressable>> GetUpdates_OLD(CancellationToken cancellationToken = default)
    //{
    //    //while (await channel.Reader.WaitToReadAsync(cancellationToken))
    //    //{
    //    //    while (channel.Reader.TryRead(out IChangeSet<TGrainObserver, IAddressable>? item))
    //    //    {
    //    //        yield return item;
    //    //    }
    //    //}

    //    // REVIEW TODO CLEANUP - replace above with this?
    //    await foreach (var item in channel.Reader.ReadAllAsync())
    //    {
    //        yield return item;
    //    }
    //    Debug.WriteLine("GetUpdates finished"); // TEMP
    //}
    //[Obsolete]
    //private readonly Channel<IChangeSet<TGrainObserver, IAddressable>> channel = Channel.CreateUnbounded<IChangeSet<TGrainObserver, IAddressable>>();

    #endregion

    #endregion
}
