#nullable enable

using LionFire.Data.Async.Gets;
using LionFire.Data.Async.Gets.Mvvm;
using LionFire.Reactive;

namespace LionFire.Data.Mvvm;

//public interface ISupportsPolling
//{
//    TimeSpan? PollDelay { get; }
//}

public interface IGetterVM<TValue>
    : IReactiveObjectEx
    , IGetterVMBase<TValue>
    , IViewModel<IGetterRxO<TValue>>
{
}



///// <summary>
///// 
///// </summary>
///// <typeparam name="TValue"></typeparam>
///// <remarks>
///// Implementers should implement IViewModel<IStatelessGets<T>>
///// </remarks>
//public interface IGetterVM<TValue>
//    : IGetterRxO<TValue> // pass-thru to Source
//    , IViewModel<IStatelessGetter<TValue>>
//    , IReactiveObjectEx
//{
//    ReactiveCommand<Unit, Task<IGetResult<TValue>>> GetIfNeeded { get; }

//}

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
