using LionFire.Data.Async.Gets;
using System.Reactive;

namespace LionFire.Mvvm;

public interface ILazilyResolvesVM<T> : IResolvesRx<T>
{
    ReactiveCommand<Unit, ILazyResolveResult<T>> ResolveIfNeeded { get; }
    bool HasValue { get; }
}

//public IResolvesRx<IEnumerable<TValue>>? ResolvesVM
//{
//    get
//    {
//        if (resolvesVM == null)
//        {
//            if (Items is ILazilyResolves<IEnumerable<TValue>> lazily)
//                resolvesVM = new LazilyResolvesCollectionVM<TValue, IEnumerable<TValue>>(lazily);
//        }
//        return resolvesVM;
//    }
//    set { resolvesVM = value; }
//}
//private IResolvesRx<IEnumerable<TValue>>? resolvesVM;

//public static class ResolvesCommandsX
//{
//    public static ReactiveCommand<Unit, IResolveResult<TValue>> CreateResolveCommand<TValue>(IGets<TValue> resolves, IObserver<IResolveResult<TValue>> resolveResults)
//        => 
//}
