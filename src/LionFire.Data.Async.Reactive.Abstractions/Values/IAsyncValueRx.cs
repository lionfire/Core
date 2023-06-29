using LionFire.Data.Gets;
using LionFire.Data.Sets;

namespace LionFire.Data.Reactive;

public interface IAsyncValueRx<T> 
    : ILazilyGetsRx<T>
    , ISetsRx<T>
{
}
