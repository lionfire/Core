using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async.Reactive;

public interface IAsyncValueRx<T> 
    : IAsyncValue<T>
    , ILazilyGetsRx<T>
    , ISetsRx<T>
{
}
