using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Reactive;

public interface IAsyncObservableForSyncObservers<T>
{
    Task<IAsyncDisposable> SubscribeAsync(IObserver<T> observer);
}
