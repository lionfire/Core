////using ObservableCollections;
//using Orleans.Streams;

//namespace LionFire.Orleans_.Streams;

//public class OrleansObserverAdapter<TInfo> : IObservable<TInfo>
//{
//    StreamSubscriptionHandle<TInfo> handle;

//    public static async Task<OrleansObserverAdapter<TInfo>> Create(Reactive.IAsyncObserver<TInfo> observer1, Orleans.Streams.IAsyncObservable<TInfo> parent)
//    {
//        var orleansAsyncObserver = new AsyncSystemObserverToOrleansStreamsAsyncObserver<TInfo>(observer1);

//        var result = new OrleansObserverAdapter<TInfo>()
//        {
//            handle = await parent.Subscribe(orleansAsyncObserver)
//        };
//        return result;
//    }

//    public IDisposable Subscribe(IObserver<TInfo> observer)
//    {
//        return new StreamSubscriptionHandleWrapper<TInfo>(handle);

//    }
//}