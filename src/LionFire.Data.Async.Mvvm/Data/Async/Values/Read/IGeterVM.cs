using LionFire.Data.Async.Gets;
using LionFire.Mvvm;
using LionFire.Reactive;
using System.Reactive;

namespace LionFire.Data.Async.Gets.Mvvm;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// Implementors should implement IViewModel<IStatelessGets<T>>
/// </remarks>
public interface IGeterVM<TValue> 
    : IGetterRxO<TValue>
    , IViewModel<IStatelessGetter<TValue>>
    , IReactiveObjectEx
{
    ReactiveCommand<Unit, Task<IGetResult<TValue>>> GetIfNeeded { get; }

}

//public IGetterRxO<IEnumerable<TValue>>? GetsVM
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
//private IGetterRxO<IEnumerable<TValue>>? getsVM;

//public static class ResolvesCommandsX
//{
//    public static ReactiveCommand<Unit, IGetResult<TValue>> CreateResolveCommand<TValue>(IGets<TValue> gets, IObserver<IGetResult<TValue>> getResults)
//        => 
//}
