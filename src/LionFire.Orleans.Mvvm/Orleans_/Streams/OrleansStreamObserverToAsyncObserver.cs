﻿//using ObservableCollections;
using Orleans.Streams;
using System.Reflection.Metadata;
using static LionFire.Reflection.GetMethodEx;

namespace LionFire.Orleans_.Streams;

//public class OrleansStreamObserverToAsyncObserver<TInfo> : System.IAsyncObservable<TInfo>
//{
//    StreamSubscriptionHandle<TInfo>? handle;
//    Orleans.Streams.IAsyncObservable<TInfo> observableStream;

//    public OrleansStreamObserverToAsyncObserver(Orleans.Streams.IAsyncObservable<TInfo> observableStream)
//    {
//        this.observableStream = observableStream;
//    }

//    public async Task<IAsyncDisposable> Subscribe(Reactive.IAsyncObserver<TInfo> observer)
//    {
//        handle = await observableStream.Subscribe(new AsyncObserverToOrleansStreamsAsyncObserver<TInfo>(observer));
//        return new StreamSubscriptionHandleAsyncWrapper<TInfo>(handle);
//    }
//}

public static class OrleansStreamObserverToAsyncObserverExtensions
{
    public static async Task<IAsyncDisposable> SubscribeAsync<T>(this Orleans.Streams.IAsyncObservable<T> observableStream, System.IAsyncObserver<T> observer)
    {
        var handle = await observableStream.SubscribeAsync(new AsyncSystemObserverToOrleansStreamsAsyncObserver<T>(observer));
        return new StreamSubscriptionHandleAsyncWrapper<T>(handle);
    }

    public static async Task<IAsyncDisposable> SubscribeAsync<T>(this Orleans.Streams.IAsyncObservable<T> observableStream, IObserver<T> observer)
    {
        var handle = await observableStream.SubscribeAsync(new SystemObserverToOrleansStreamsAsyncObserver<T>(observer));
        return new StreamSubscriptionHandleAsyncWrapper<T>(handle);
    }
}