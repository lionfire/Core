
using DynamicData;
using LionFire.Collections;
using LionFire.DependencyInjection;
using LionFire.Orleans_.Reactive_;
using LionFire.Resolves;
using LionFire.Structures;
using LionFire.Subscribing;
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Streams;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static LionFire.Reflection.GetMethodEx;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LionFire.Orleans_.Collections;


/// <summary>
/// Implemented with Dictionary (so keys must be unique) but you add/remove items, and a Func<TItem, TKey> is used to extract the key from the item.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public class KeyedCollectionG<TKey, TItem> : Grain
    , IKeyedCollectionG<TKey, TItem>
    //, IAsyncCreating<TNotificationItem> 
    //, ICreatingAsyncDictionary<string, TNotificationItem>
    where TKey : notnull
{
    #region Dependencies

    public IClusterClient ClusterClient => ServiceProvider.GetRequiredService<IClusterClient>();
    public ILogger<KeyedCollectionG<TKey, TItem>> Logger { get; }

    public Func<TItem, TKey> KeySelector { get; }

    #endregion

    #region State (persisted)

    protected IPersistentState<Dictionary<TKey, TItem>> ItemsState { get; }


    public Task<IEnumerable<TItem>> Items() => Task.FromResult<IEnumerable<TItem>>(ItemsState.State.Values.ToArray());

    #endregion

    #region Configuration

    public static bool DefaultAwaitPublishingNotificationEvents { get; set; } = false;
    public bool AwaitPublishingNotificationEvents { get; set; } = DefaultAwaitPublishingNotificationEvents;

    #endregion

    #region Lifecycle

    public static Func<TItem, TKey>? staticKeySelector = AddressableKeySelectorProvider.TryGetKeySelector<TItem, TKey>();

    // TODO: Replace List<TNotificationItem> with Dictionary<string, TNotificationItem>
    public KeyedCollectionG(IServiceProvider serviceProvider, /* [PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)] */ IPersistentState<Dictionary<TKey, TItem>> items, ILogger<KeyedCollectionG<TKey, TItem>> logger)
    {
        ItemsState = items;
        Logger = logger;

        grainObserverManager = new(() => new GrainObserverManager<ChangeSet<TItem, TKey>>(this, DefaultGrainObserverTimeout));

        KeySelector = staticKeySelector ??= KeySelectors.GetKeySelector<TItem, TKey>(serviceProvider);
    }


    public override Task OnActivateAsync(CancellationToken cancellationToken = default)
    {
        ItemsState.State ??= new();
        return base.OnActivateAsync(cancellationToken);
    }

    #endregion

    #region IGrainObservers

    protected GrainObserverManager<ChangeSet<TItem, TKey>> GrainObserverManager => grainObserverManager.Value;
    Lazy<GrainObserverManager<ChangeSet<TItem, TKey>>> grainObserverManager;

    public static TimeSpan DefaultGrainObserverTimeout => TimeSpan.FromMinutes(5);

    public ValueTask<TimeSpan> SubscriptionTimeout() => ValueTask.FromResult(GrainObserverManager.SubscriptionTimeout);

    public ValueTask Subscribe(IAsyncObserverO<ChangeSet<TItem, TKey>> subscriber)
    {
        GrainObserverManager.Subscribe(subscriber);
        return ValueTask.CompletedTask;
    }
    public ValueTask Unsubscribe(IAsyncObserverO<ChangeSet<TItem, TKey>> subscriber)
    {
        GrainObserverManager.Unsubscribe(subscriber);
        return ValueTask.CompletedTask;
    }

    public Task NotifyObservers(ChangeSet<TItem, TKey> message)
        => GrainObserverManager.NotifyObservers(message);

    #endregion

    #region Instantiation

    IKeyGenerator<TKey>? KeyGenerator = typeof(TKey) == typeof(string) ? (IKeyGenerator<TKey>?)new KeyGenerator() : null;

    protected SortedList<int, Stack<TKey>> free = new(); // FUTURE - TODO: upon delete, add keys here (after time expiry?)
    protected TKey InstantiateKey()
    {
        //return Guid.NewGuid().ToString();
        return (KeyGenerator ?? throw new ArgumentNullException(nameof(KeyGenerator))).Next(ItemsState.State.Keys, free);
    }

    protected virtual void OnInstantiated(TItem value) { }


    protected virtual TItem Instantiate(Type type)
    {
        //return GrainFactory.GetGrain(type, InstantiateKey());
#if DEBUG
        if (type.IsAssignableTo(typeof(IGrain))) throw new NotSupportedException($"{nameof(Instantiate)} should be overridden for grain types");
#endif
        var key = InstantiateKey();

        //TKey key;
        //do
        //{            
        //    key = KeyGenerator.Next((ICollection<string>)ItemsState.State.Keys, free).Create<TKey>();
        //} while (ItemsState.State.ContainsKey(key));

        if (ActivatorUtilitiesEx.TryCreateInstance(ServiceProvider, out TItem instance, type)) { return instance; }
        return (TItem)ActivatorUtilities.CreateInstance(ServiceProvider, type, key, type);
    }

    #endregion

    #region List: read

    public Task<int> Count => Task.FromResult(ItemsState.State.Count);
    public Task<int> GetCount() => Task.FromResult(ItemsState.State.Count);

    public Task<bool> IsReadOnly => Task.FromResult(false);
    public Task<bool> GetIsReadOnly() => IsReadOnly;

    public Task<bool> Contains(TItem item) => Task.FromResult(ItemsState.State.ContainsValue(item));
    public Task<bool> ContainsKey(TKey key) => Task.FromResult(ItemsState.State.ContainsKey(key));

    public Task<IEnumerable<TItem>> GetEnumerableAsync() => Task.FromResult<IEnumerable<TItem>>(ItemsState.State.Values.ToArray());

    #endregion

    #region List: write

    #region Create

    public async Task<TItem> Create(Type type, params object[] constructorParameters)
    {
        if (constructorParameters != null && constructorParameters.Length > 0)
        {
            throw new NotSupportedException($"{nameof(constructorParameters)} not supported on Grains");
        }
        var item = Instantiate(type);
        OnInstantiated(item);
        await Add(item);
        return item;
    }

    // Not accessible via Orleans
    public async virtual Task<TItem> CreateWithInitializer(Type type, Action<TItem>? init = null)
    {
        var item = Instantiate(type);

        OnInstantiated(item);
        init?.Invoke(item);

        await Add(item);

        return item;
    }

    #endregion

    #region Insert
    //public Task Insert(int index, TItem item)
    //{
    //    ItemsState.State.Insert(index, item);
    //    return ItemsState.WriteStateAsync();
    //}
    #endregion

    #region Add

    public async Task Add(TItem item)
    {
        var key = KeySelector(item);
        ItemsState.State.Add(key, item);
        await ItemsState.WriteStateAsync();

        await PublishCollectionChanged(new ChangeSet<TItem, TKey>(new Change<TItem, TKey>[] { new Change<TItem, TKey>(ChangeReason.Add, key, item) }));
    }

    #endregion

    #region Remove
    public async virtual Task Clear()
    {
        var changeSet = new ChangeSet<TItem, TKey>(
                ItemsState.State.Select(kvp => new Change<TItem, TKey>(ChangeReason.Remove, kvp.Key, kvp.Value)));

        ItemsState.State.Clear();
        await ItemsState.WriteStateAsync();

        await PublishCollectionChanged(changeSet);
    }

    public async virtual Task<bool> Remove(TKey key)
    {
        if (!ItemsState.State.TryGetValue(key, out var item)) return false;

        var result = ItemsState.State.Remove(key);
        await ItemsState.WriteStateAsync();

        await PublishCollectionChanged(new ChangeSet<TItem, TKey>(new Change<TItem, TKey>[] { new(ChangeReason.Remove, key, item) }));

        return result;
    }

    public Task<bool> Remove(TItem item) => Remove(KeySelector(item));

    #endregion

    #endregion

    #region Event: CollectionChanged

    #region Stream

    private IAsyncStream<ChangeSet<TItem, TKey>> CollectionChangedStream => this.GetStreamProvider("ChangeNotifications").GetStream<ChangeSet<TItem, TKey>>(
               this.GetPrimaryKeyString(), "CollectionChanged");

    #endregion

    public async Task PublishCollectionChanged(ChangeSet<TItem, TKey> args)
    {
        //var s = ClusterClient.GetStreamProvider("ChangeNotifications").GetStream<NotifyCollectionChangedEventArgs<TNotificationItem>>(Guid.Parse(Source.GetPrimaryKeyString()), "CollectionChanged");

        //IStreamProvider streamProvider = GetStreamProvider("ChangeNotifications");

        //var deviceStream = streamProvider.GetStream<CollectionChangeEventArgs>(
        //       Guid.Parse(this.GetPrimaryKeyString()), "CollectionChanged");

        if (grainObserverManager.IsValueCreated) { await GrainObserverManager.NotifyObservers(args); }

        var publishTask = CollectionChangedStream.OnNextAsync(args);
        if (AwaitPublishingNotificationEvents) { await publishTask; }
        else
        {
            Task.Run(async () =>
            {
                try
                {
                    await publishTask;
                }
                catch (Exception ex)
                {
                    publishErrors.OnNext(ex);
                }
            }).FireAndForget();
        }
    }

    public IObservable<Exception> PublishErrors => publishErrors.AsObservable();
    Subject<Exception> publishErrors = new();

    //// REVIEW - shouldn't be necessary, but might help if things get out of sync.
    //// UNTESTED - Not sure it works as intended
    //public Task Reset()
    //    => PublishCollectionChanged(new ChangeSet<TItem, TKey>(ItemsState.State.Select(s => new Change<TItem, TKey> { Reason = ChangeReason.Refresh })));

    #endregion

    public virtual Task<IEnumerable<Type>> SupportedTypes() => Task.FromResult(Enumerable.Empty<Type>());

    public Task<IResolveResult<IEnumerable<TItem>>> Resolve()
        => Task.FromResult<IResolveResult<IEnumerable<TItem>>>(new ResolveResultSuccess<IEnumerable<TItem>>(ItemsState.State.Values.ToArray()));


}
