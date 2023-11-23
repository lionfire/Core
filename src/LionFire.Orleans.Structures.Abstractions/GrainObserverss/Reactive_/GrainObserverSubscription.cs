
using LionFire.Orleans_.Reactive_;
using System.Reactive;
using System.Reactive.Subjects;

namespace LionFire.Orleans_.Collections;

// OLD - folded into KeyedListObserverO
/// <summary>
/// This is owned by the Observer, not the Observable.  It is only created once when initially subscribing.
/// </summary>
/// <typeparam name="T"></typeparam>
//[GenerateSerializer]
//internal class GrainObserverSubscription<T> : IAsyncDisposable
//{
//    [Id(0)]
//    public IAsyncObserverO<T>? Subscriber { get; set; }
//    [Id(1)]
//    public IGrainObservableG<T>? Observable { get; set; }

//    public async ValueTask DisposeAsync()
//    {
//        if (Subscriber != null && Observable != null)
//        {
//            var s = Subscriber;
//            Subscriber = null;
//            var o = Observable;
//            Observable = null;
//            await o.Unsubscribe(s).ConfigureAwait(false);
//        }
//        var task = Disposing?.Invoke();
//        if (task != null) await task.ConfigureAwait(false);
//    }

//    public Func<Task>? Disposing { get; set; }
//}

// TRIAGE - duplicate implementation

//[GenerateSerializer]
//internal class UnsubscribeOnDispose : IAsyncDisposable, IDependsOn<IGrainFactory>
//{
//    [Id(0)]
//    public GrainId Publisher { get; set; }

//    [Id(1)]
//    public IAddressable? Subscriber { get; set; }

//    public IGrainFactory? GrainFactory { get; set; }
//    IGrainFactory IDependsOn<IGrainFactory>.Dependency { set => GrainFactory = value; }

//    public ValueTask DisposeAsync()
//    {
//        ArgumentNullException.ThrowIfNull(GrainFactory, "IDependsOn<IGrainFactory>.Dependency");
//        ArgumentNullException.ThrowIfNull(Subscriber);
//        var grain = (IChangeSetObservableBaseG)GrainFactory.GetGrain(Publisher);
//        grain.UnsubscribeAsync(Subscriber);
//        return ValueTask.CompletedTask;
//    }
//}
