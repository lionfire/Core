using LionFire.Data.Gets;
using LionFire.Mvvm;
using System.Reactive;

namespace LionFire.Data.Mvvm;

public interface ILazilyGetsVM<T> 
    : IGetsVM<T>
    , IViewModel<ILazilyGets<T>>
{
    ReactiveCommand<Unit, ILazyGetResult<T>> GetIfNeeded { get; }
    //bool HasValue { get; }

    //ReactiveCommand<Unit, Unit> DiscardValue { get; }
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
