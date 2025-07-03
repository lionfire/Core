using DynamicData;
using Orleans;
using Orleans.Runtime;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LionFire.Orleans_;

public interface IGrainObserverRegistrar<TGrainObserver> //: IGrainWithStringKey
    where TGrainObserver : notnull
{
    ValueTask<TimeSpan> RegistrationTimeout();
    Task Register(TGrainObserver worker);
    ValueTask Unregister(TGrainObserver worker);

    ValueTask<IEnumerable<TGrainObserver>> GetAll();
    IAsyncEnumerable<IChangeSet<TGrainObserver, IAddressable>> GetUpdates(CancellationToken cancellationToken = default);

}

public static class GrainObserverExX
{
    public static async ValueTask<GrainObserverEx<TGrainObserver>> RegisterEx<TGrainObserver>(this IGrainObserverRegistrar<TGrainObserver> grainObserverRegistrar, TGrainObserver observer)
        where TGrainObserver : notnull
    {
        var timeout = await grainObserverRegistrar.RegistrationTimeout();
        return new GrainObserverEx<TGrainObserver>(observer, timeout, grainObserverRegistrar);
    }
}

public class GrainObserverEx<TGrainObserver> : IGrainObserver, IAsyncDisposable
        where TGrainObserver : notnull
{
    #region Relationships

    public TGrainObserver Observer { get; }
    public IGrainObserverRegistrar<TGrainObserver> GrainObserverRegistrar { get; }

    #endregion

    #region Parameters

    public TimeSpan Timeout { get; }

    #endregion

    #region Lifecycle

    public GrainObserverEx(TGrainObserver observer, TimeSpan timeout, IGrainObserverRegistrar<TGrainObserver> grainObserverRegistrar)
    {
        Observer = observer;
        Timeout = timeout;
        GrainObserverRegistrar = grainObserverRegistrar;

        var renewTime = Timeout switch
        {
            var t when t > TimeSpan.FromMinutes(5) => Timeout - TimeSpan.FromSeconds(65),
            var t when t < TimeSpan.FromMilliseconds(2000) => Timeout * 0.5,
            _ => Timeout * 0.8
        };


        Task.Run(async () =>
        {
            Console.WriteLine($"GrainObserverEx<{typeof(TGrainObserver).Name}> Expiration time: {Timeout}.  Using renew interval of {renewTime}");
            var PeriodicTimer = new PeriodicTimer(renewTime);

            while (!CancellationTokenSource.IsCancellationRequested)
            {
                Console.WriteLine($"GrainObserverEx<{typeof(TGrainObserver).Name}> registering...");
                var sw = Stopwatch.StartNew();
                await GrainObserverRegistrar.Register(Observer);
                Console.WriteLine($"GrainObserverEx<{typeof(TGrainObserver).Name}> registering...done. ({sw.ElapsedMilliseconds}ms)");

                await PeriodicTimer.WaitForNextTickAsync(CancellationTokenSource.Token);
            }
        });
    }

    public async ValueTask DisposeAsync()
    {
        CancellationTokenSource.Cancel();
        //PeriodicTimer.Dispose();

        Console.WriteLine($"GrainObserverEx<{typeof(TGrainObserver).Name}> unregistering...");
        var sw = Stopwatch.StartNew();
        await GrainObserverRegistrar.Unregister(Observer);
        Console.WriteLine($"GrainObserverEx<{typeof(TGrainObserver).Name}> unregistering...done. ({sw.ElapsedMilliseconds}ms)");
    }

    #endregion

    #region State

    //PeriodicTimer PeriodicTimer;
    CancellationTokenSource CancellationTokenSource = new();

    #endregion

}

//public interface IObservableGrainObserverRegistrar<TGrainObserver> : IGrainObserverRegistrar<TGrainObserver>
//{
//#error NEXT: do I have a grain observer interface yet?
//}
