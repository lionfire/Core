using DynamicData;
using LionFire.Orleans_;
using LionFire.Orleans_.Collections;
using LionFire.Orleans_.Utilities;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Utilities;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;

namespace LionFire.Valor.Universes.Sim.Workers.Grains;

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
    public GrainObserverRegistrar(IServiceProvider serviceProvider, ILogger logger) : base(serviceProvider, logger, e => e.Observer)
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
