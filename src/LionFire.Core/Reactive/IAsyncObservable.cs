using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Reactive;

public interface IAsyncObservable<T>
{
    Task<IAsyncDisposable> SubscribeAsync(IAsyncObserver<T> observer);
}

public interface IAsyncObservableForSyncObservers<T>
{
    Task<IAsyncDisposable> SubscribeAsync(IObserver<T> observer);
}
