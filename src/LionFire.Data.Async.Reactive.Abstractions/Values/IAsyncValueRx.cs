using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async.Reactive;

public interface IAsyncValueRx<T> 
    : IStatelessAsyncValue<T>
    , ILazilyGetsRx<T>
    , ISetsRx<T>
{
}
