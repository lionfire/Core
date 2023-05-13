using DynamicData;
using LionFire.Collections;
using LionFire.Collections.Async;
using Orleans;
using Orleans.Runtime;

namespace LionFire.Orleans_.Collections;

public interface IGrainObserverO<T> 
    : System.IAsyncObserver<T>
    , IGrainObserver
    , IAsyncDisposable
{    
}

