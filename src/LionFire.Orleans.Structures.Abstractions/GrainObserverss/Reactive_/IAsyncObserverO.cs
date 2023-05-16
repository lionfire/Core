namespace LionFire.Orleans_.Reactive_;

/// <summary>
/// An Orleans IGrainObserver that acts as an IAsyncObserver<TValue>
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// Orleans Streams has an interface almost identical to System.IAsyncObserver, with the exception that the OnNext method also provides a cursor that can be used to reattach at a particular point in the stream.
/// </remarks>
public interface IAsyncObserverO<TValue>
    : System.IAsyncObserver<TValue>
    , System.IAsyncObservable<TValue>
    , IGrainObserver
{

}
