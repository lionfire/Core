////using ObservableCollections;
//using Orleans.Streams;

//namespace LionFire.Orleans_.Streams;

//public class OrleansObserverAdapter<TValue> : IObservable<TValue>
//{
//    StreamSubscriptionHandle<TValue> handle;

//    public static async Task<OrleansObserverAdapter<TValue>> Create(Reactive.IAsyncObserver<TValue> observer1, Orleans.Streams.IAsyncObservable<TValue> parent)
//    {
//        var orleansAsyncObserver = new AsyncSystemObserverToOrleansStreamsAsyncObserver<TValue>(observer1);

//        var result = new OrleansObserverAdapter<TValue>()
//        {
//            handle = await parent.Subscribe(orleansAsyncObserver)
//        };
//        return result;
//    }

//    public IDisposable Subscribe(IObserver<TValue> observer)
//    {
//        return new StreamSubscriptionHandleWrapper<TValue>(handle);

//    }
//}