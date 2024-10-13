using LionFire.Orleans_.Reactive_;
using LionFire.Data.Async.Gets;
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using DynamicData;
using Orleans.Streams;
using LionFire.Orleans_.ObserverGrains;
using System.Runtime.CompilerServices;
using LionFire.Data.Async;
using Orleans;

namespace LionFire.Orleans_.Collections;

/// <summary>
/// RENAME: KeyablesDictionary
/// This is a keyed collection, not a dictionary, because TValues must be able to specify their own key.
/// However, like a dictionary, keys must be unique, so duplicate TValues are effectively not allowed.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public abstract class KeyedCollectionGBase<TKey, TValue>
    : Grain
    , IKeyedCollectionG<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{

    #region Dependencies

    //public IClusterClient ClusterClient => ServiceProvider.GetRequiredService<IClusterClient>();
    public ILogger Logger { get; }

    public Func<TValue, TKey> KeySelector { get; }

    #endregion

    #region State (persisted)

    public Task<IEnumerable<TValue>> Items() => Task.FromResult<IEnumerable<TValue>>(ItemsDictionary.Items.ToArray());

    protected IObservableCache<TValue, TKey> ItemsDictionary => items;
    protected SourceCache<TValue, TKey> items { get; }

    #endregion

    #region Configuration

    public static bool DefaultAwaitPublishingNotificationEvents { get; set; } = false;
    public bool AwaitPublishingNotificationEvents { get; set; } = DefaultAwaitPublishingNotificationEvents;

    #endregion

    #region Lifecycle

    public static Func<TValue, TKey>? staticKeySelector = AddressableKeySelectorProvider.TryGetKeySelector<TValue, TKey>();

    // TODO: Replace List<TNotificationItem> with Dictionary<string, TNotificationItem>
    public KeyedCollectionGBase(IServiceProvider serviceProvider, ILogger logger, Func<TValue, TKey>? keySelector = null)
    {
        Logger = logger;

        grainObserverManager = new(() => new AsyncObserverGrainObserverManager<ChangeSet<TValue, TKey>>(this, DefaultGrainObserverTimeout));

        KeySelector = keySelector 
            ?? (staticKeySelector ??= KeySelectors<TValue, TKey>.GetKeySelector(serviceProvider));
        items = new(KeySelector);
    }


    #endregion

    #region IGrainObservers

    protected AsyncObserverGrainObserverManager<ChangeSet<TValue, TKey>> GrainObserverManager => grainObserverManager.Value;
    Lazy<AsyncObserverGrainObserverManager<ChangeSet<TValue, TKey>>> grainObserverManager;

    public static TimeSpan DefaultGrainObserverTimeout => TimeSpan.FromMinutes(5);

    public ValueTask<TimeSpan> SubscriptionTimeout() => ValueTask.FromResult(GrainObserverManager.SubscriptionTimeout);

    public ValueTask<TimeSpan> Subscribe(IAsyncObservableO<ChangeSet<TValue, TKey>> subscriber)
    {
        GrainObserverManager.Subscribe(subscriber);
        return ValueTask.FromResult(GrainObserverManager.SubscriptionTimeout);
    }
    public ValueTask Unsubscribe(IAsyncObservableO<ChangeSet<TValue, TKey>> subscriber)
    {
        GrainObserverManager.Unsubscribe(subscriber);
        return ValueTask.CompletedTask;
    }

    public Task NotifyObservers(ChangeSet<TValue, TKey> message)
        => GrainObserverManager.NotifyObservers(message);

    #endregion

    #region Instantiation

    IKeyGenerator<TKey>? KeyGenerator = typeof(TKey) == typeof(string) ? (IKeyGenerator<TKey>?)new KeyGenerator() : null;

    protected SortedList<int, Stack<TKey>> free = new(); // FUTURE - TODO: upon delete, add keys here (after time expiry?)
    protected TKey InstantiateKey()
    {
        //return Guid.NewGuid().ToString();
        return (KeyGenerator ?? throw new ArgumentNullException(nameof(KeyGenerator))).Next(ItemsDictionary.Keys, free);
    }

    protected virtual void OnInstantiated(TValue value) { }


    protected virtual TValue Instantiate(Type type)
    {
        //return GrainFactory.GetGrain(type, InstantiateKey());
#if DEBUG
        if (type.IsAssignableTo(typeof(IGrain))) throw new NotSupportedException($"{nameof(Instantiate)} should be overridden for grain types");
#endif
        var key = InstantiateKey();

        //TKey key;
        //do
        //{            
        //    key = KeyGenerator.Next((ICollection<string>)ItemsDictionary.Keys, free).Create<TKey>();
        //} while (ItemsDictionary.ContainsKey(key));

        if (ActivatorUtilitiesEx.TryCreateInstance(ServiceProvider, out TValue instance, type)) { return instance; }
        return (TValue)ActivatorUtilities.CreateInstance(ServiceProvider, type, key, type);
    }

    #endregion

    #region List: read

    public Task<int> Count => Task.FromResult(ItemsDictionary.Count);
    public Task<int> GetCount() => Task.FromResult(ItemsDictionary.Count);

    public Task<bool> IsReadOnly => Task.FromResult(false);
    public Task<bool> GetIsReadOnly() => IsReadOnly;

    public Task<bool> Contains(TValue item) => Task.FromResult(ItemsDictionary.Items.Contains(item)); // OPTIMIZE: If Dictionary, use ContainsValue
    public Task<bool> ContainsKey(TKey key) => Task.FromResult(ItemsDictionary.Lookup(key).HasValue);

    public Task<IEnumerable<TValue>> GetEnumerableAsync() => Task.FromResult<IEnumerable<TValue>>(ItemsDictionary.Items.ToArray());

    #endregion

    #region List: write

    #region Create

    public async Task<TValue> Create(Type type, params object[]? constructorParameters)
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
    public async virtual Task<TValue> CreateWithInitializer(Type type, Action<TValue>? init = null)
    {
        var item = Instantiate(type);

        OnInstantiated(item);
        init?.Invoke(item);

        await Add(item);

        return item;
    }

    #endregion

    #region Insert
    //public Task Insert(int index, TValue item)
    //{
    //    ItemsDictionary.Insert(index, item);
    //    return ItemsState.WriteStateAsync();
    //}
    #endregion

    protected virtual ValueTask OnStateHasChanged() => ValueTask.CompletedTask;

    #region Add

    public async Task Add(TValue item)
    {
        items.AddOrUpdate(item);
        await OnStateHasChanged();

        //var key = KeySelector(item);
        //await PublishCollectionChanged(new ChangeSet<TValue, TKey>(new Change<TValue, TKey>[] { new Change<TValue, TKey>(ChangeReason.Add, key, item) }));
    }

    #endregion

    #region Remove
    public async virtual Task Clear()
    {
        //var changeSet = new ChangeSet<TValue, TKey>(
                //ItemsDictionary.KeyValues.Select(kvp => new Change<TValue, TKey>(ChangeReason.Remove, kvp.Key, kvp.Value)));

        items.Clear();
        await OnStateHasChanged();

        //await PublishCollectionChanged(changeSet);
    }

    public async virtual Task<bool> Remove(TKey key)
    {
        var lookup = items.Lookup(key);
        if (!lookup.HasValue) return false;
        items.Remove(key);
        await OnStateHasChanged();

        //await PublishCollectionChanged(new ChangeSet<TValue, TKey>(new Change<TValue, TKey>[] { new(ChangeReason.Remove, key, item) }));

        return true;
    }

    public Task<bool> Remove(TValue item) => Remove(KeySelector(item));

    #endregion

    #endregion

    #region Event: CollectionChanged

    #region Stream

    private IAsyncStream<ChangeSet<TValue, TKey>> CollectionChangedStream => this.GetStreamProvider("ChangeNotifications").GetStream<ChangeSet<TValue, TKey>>(
               this.GetPrimaryKeyString(), "CollectionChanged");

    #endregion

    // OLD
    //public async Task PublishCollectionChanged(ChangeSet<TValue, TKey> args)
    //{
    //    //var s = ClusterClient.GetStreamProvider("ChangeNotifications").GetStream<NotifyCollectionChangedEventArgs<TNotificationItem>>(Guid.Parse(Source.GetPrimaryKeyString()), "CollectionChanged");

    //    //IStreamProvider streamProvider = GetStreamProvider("ChangeNotifications");

    //    //var deviceStream = streamProvider.GetStream<CollectionChangeEventArgs>(
    //    //       Guid.Parse(this.GetPrimaryKeyString()), "CollectionChanged");

    //    if (grainObserverManager.IsValueCreated) { await GrainObserverManager.NotifyObservers(args); }

    //    var publishTask = CollectionChangedStream.OnNextAsync(args);
    //    if (AwaitPublishingNotificationEvents) { await publishTask; }
    //    else
    //    {
    //        Task.Run(async () =>
    //        {
    //            try
    //            {
    //                await publishTask;
    //            }
    //            catch (Exception ex)
    //            {
    //                publishErrors.OnNext(ex);
    //            }
    //        }).FireAndForget();
    //    }
    //}

    public IObservable<Exception> PublishErrors => publishErrors.AsObservable();
    Subject<Exception> publishErrors = new();

    //// REVIEW - shouldn't be necessary, but might help if things get out of sync.
    //// UNTESTED - Not sure it works as intended
    //public Task Reset()
    //    => PublishCollectionChanged(new ChangeSet<TValue, TKey>(ItemsDictionary.Select(s => new Change<TValue, TKey> { Reason = ChangeReason.Refresh })));

    #endregion

    public virtual Task<IEnumerable<Type>> SupportedTypes() => Task.FromResult(Enumerable.Empty<Type>());

    public Task<IGetResult<IEnumerable<TValue>>> Get(CancellationToken cancellationToken = default)
        => Task.FromResult<IGetResult<IEnumerable<TValue>>>(new SuccessGetResult<IEnumerable<TValue>>(ItemsDictionary.Items.ToArray()));


}

#if MAYBE
public static class DynamicDataX
{
    public static bool TryGetValue<TValue, TKey>(this IObservableCache<TValue, TKey> oc, TKey key, out TValue result)
    {
        var lookup = oc.Lookup(key);
        if (lookup.HasValue)
        {
            result = lookup.Value;
        }
        else { result = default!; }
        return lookup.HasValue;
    }
}
#endif

#if DUPE
public abstract class KeyedCollectionGBase<TKey, TValue>
    : Grain
    , IKeyedCollectionG<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{

    #region Dependencies

    public IClusterClient ClusterClient => ServiceProvider.GetRequiredService<IClusterClient>();
    public ILogger<KeyedCollectionG<TKey, TValue>> Logger { get; }

    public Func<TValue, TKey> KeySelector { get; }

    #endregion

    #region State (persisted)

    public Task<IEnumerable<TValue>> Items() => Task.FromResult<IEnumerable<TValue>>(ItemsState.State.Values.ToArray());

    #endregion

    #region Configuration

    public static bool DefaultAwaitPublishingNotificationEvents { get; set; } = false;
    public bool AwaitPublishingNotificationEvents { get; set; } = DefaultAwaitPublishingNotificationEvents;

    #endregion

    #region Lifecycle

    public static Func<TValue, TKey>? staticKeySelector = AddressableKeySelectorProvider.TryGetKeySelector<TValue, TKey>();

    // TODO: Replace List<TNotificationItem> with Dictionary<string, TNotificationItem>
    public KeyedCollectionG(IServiceProvider serviceProvider, /* [PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)] */ IPersistentState<Dictionary<TKey, TValue>> items, ILogger<KeyedCollectionGBase<TKey, TValue>> logger)
    {
        Logger = logger;

        grainObserverManager = new(() => new AsyncObserverGrainObserverManager<ChangeSet<TValue, TKey>>(this, DefaultGrainObserverTimeout));

        KeySelector = staticKeySelector ??= KeySelectors<TValue, TKey>.GetKeySelector(serviceProvider);
    }


    public override Task OnActivateAsync(CancellationToken cancellationToken = default)
    {
        ItemsState.State ??= new();
        return base.OnActivateAsync(cancellationToken);
    }

    #endregion

    #region IGrainObservers

    protected AsyncObserverGrainObserverManager<ChangeSet<TValue, TKey>> GrainObserverManager => grainObserverManager.Value;
    Lazy<AsyncObserverGrainObserverManager<ChangeSet<TValue, TKey>>> grainObserverManager;

    public static TimeSpan DefaultGrainObserverTimeout => TimeSpan.FromMinutes(5);

    public ValueTask<TimeSpan> SubscriptionTimeout() => ValueTask.FromResult(GrainObserverManager.SubscriptionTimeout);

    public ValueTask<TimeSpan> Subscribe(IAsyncObservableO<ChangeSet<TValue, TKey>> subscriber)
    {
        GrainObserverManager.Subscribe(subscriber);
        return ValueTask.FromResult(GrainObserverManager.SubscriptionTimeout);
    }
    public ValueTask Unsubscribe(IAsyncObservableO<ChangeSet<TValue, TKey>> subscriber)
    {
        GrainObserverManager.Unsubscribe(subscriber);
        return ValueTask.CompletedTask;
    }

    public Task NotifyObservers(ChangeSet<TValue, TKey> message)
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

    protected virtual void OnInstantiated(TValue value) { }


    protected virtual TValue Instantiate(Type type)
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

        if (ActivatorUtilitiesEx.TryCreateInstance(ServiceProvider, out TValue instance, type)) { return instance; }
        return (TValue)ActivatorUtilities.CreateInstance(ServiceProvider, type, key, type);
    }

    #endregion

    #region List: read

    public Task<int> Count => Task.FromResult(ItemsState.State.Count);
    public Task<int> GetCount() => Task.FromResult(ItemsState.State.Count);

    public Task<bool> IsReadOnly => Task.FromResult(false);
    public Task<bool> GetIsReadOnly() => IsReadOnly;

    public Task<bool> Contains(TValue item) => Task.FromResult(ItemsState.State.ContainsValue(item));
    public Task<bool> ContainsKey(TKey key) => Task.FromResult(ItemsState.State.ContainsKey(key));

    public Task<IEnumerable<TValue>> GetEnumerableAsync() => Task.FromResult<IEnumerable<TValue>>(ItemsState.State.Values.ToArray());

    #endregion

    #region List: write

    #region Create

    public async Task<TValue> Create(Type type, params object[]? constructorParameters)
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
    public async virtual Task<TValue> CreateWithInitializer(Type type, Action<TValue>? init = null)
    {
        var item = Instantiate(type);

        OnInstantiated(item);
        init?.Invoke(item);

        await Add(item);

        return item;
    }

    #endregion

    #region Insert
    //public Task Insert(int index, TValue item)
    //{
    //    ItemsState.State.Insert(index, item);
    //    return ItemsState.WriteStateAsync();
    //}
    #endregion

    #region Add

    public async Task Add(TValue item)
    {
        var key = KeySelector(item);
        ItemsState.State.Add(key, item);
        await ItemsState.WriteStateAsync();

        await PublishCollectionChanged(new ChangeSet<TValue, TKey>(new Change<TValue, TKey>[] { new Change<TValue, TKey>(ChangeReason.Add, key, item) }));
    }

    #endregion

    #region Remove
    public async virtual Task Clear()
    {
        var changeSet = new ChangeSet<TValue, TKey>(
                ItemsState.State.Select(kvp => new Change<TValue, TKey>(ChangeReason.Remove, kvp.Key, kvp.Value)));

        ItemsState.State.Clear();
        await ItemsState.WriteStateAsync();

        await PublishCollectionChanged(changeSet);
    }

    public async virtual Task<bool> Remove(TKey key)
    {
        if (!ItemsState.State.TryGetValue(key, out var item)) return false;

        var result = ItemsState.State.Remove(key);
        await ItemsState.WriteStateAsync();

        await PublishCollectionChanged(new ChangeSet<TValue, TKey>(new Change<TValue, TKey>[] { new(ChangeReason.Remove, key, item) }));

        return result;
    }

    public Task<bool> Remove(TValue item) => Remove(KeySelector(item));

    #endregion

    #endregion

    #region Event: CollectionChanged

    #region Stream

    private IAsyncStream<ChangeSet<TValue, TKey>> CollectionChangedStream => this.GetStreamProvider("ChangeNotifications").GetStream<ChangeSet<TValue, TKey>>(
               this.GetPrimaryKeyString(), "CollectionChanged");

    #endregion

    public async Task PublishCollectionChanged(ChangeSet<TValue, TKey> args)
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
    //    => PublishCollectionChanged(new ChangeSet<TValue, TKey>(ItemsState.State.Select(s => new Change<TValue, TKey> { Reason = ChangeReason.Refresh })));

    #endregion

    public virtual Task<IEnumerable<Type>> SupportedTypes() => Task.FromResult(Enumerable.Empty<Type>());

    public Task<IGetResult<IEnumerable<TValue>>> Get(CancellationToken cancellationToken = default)
        => Task.FromResult<IGetResult<IEnumerable<TValue>>>(new SuccessGetResult<IEnumerable<TValue>>(ItemsState.State.Values.ToArray()));


}

#endif