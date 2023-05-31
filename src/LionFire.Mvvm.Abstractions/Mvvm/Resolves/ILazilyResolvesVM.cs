using LionFire.Resolves;
using System.Reactive;

namespace LionFire.Mvvm;

public interface ILazilyResolvesVM<T> : IResolvesVM<T>
{
    ReactiveCommand<Unit, ILazyResolveResult<T>> ResolveIfNeeded { get; }
    bool HasValue { get; }
}

//public IResolvesVM<IEnumerable<TValue>>? ResolvesVM
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
//private IResolvesVM<IEnumerable<TValue>>? resolvesVM;

//public static class ResolvesCommandsX
//{
//    public static ReactiveCommand<Unit, IResolveResult<TValue>> CreateResolveCommand<TValue>(IResolves<TValue> resolves, IObserver<IResolveResult<TValue>> resolveResults)
//        => 
//}
