using LionFire.Data.Gets;
using LionFire.Mvvm;
using LionFire.Reactive;
using System.Reactive;

namespace LionFire.Data.Gets.Mvvm;



/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Implementors should implement IViewModel<IStatelessGets<T>>
/// </remarks>
public interface IGetsVM<T> 
    : IGetsRx<T>
    , IViewModel<IStatelessGets<T>>
    , IReactiveObjectEx
{
    ReactiveCommand<Unit, Task<ILazyGetResult<T>>> GetIfNeeded { get; }

}

//public IGetsRx<IEnumerable<TValue>>? GetsVM
//{
//    get
//    {
//        if (getsVM == null)
//        {
//            if (Items is ILazilyGets<IEnumerable<TValue>> lazily)
//                getsVM = new LazilyResolvesCollectionVM<TValue, IEnumerable<TValue>>(lazily);
//        }
//        return getsVM;
//    }
//    set { getsVM = value; }
//}
//private IGetsRx<IEnumerable<TValue>>? getsVM;

//public static class ResolvesCommandsX
//{
//    public static ReactiveCommand<Unit, IGetResult<TValue>> CreateResolveCommand<TValue>(IGets<TValue> gets, IObserver<IGetResult<TValue>> getResults)
//        => 
//}
