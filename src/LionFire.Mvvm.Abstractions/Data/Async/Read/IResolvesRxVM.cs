using LionFire.Data.Async.Gets;
using System.Reactive;

namespace LionFire.Data.Async.Mvvm;

public interface IResolvesRxVM<T> : IResolvesRx<T>
{
    new ReactiveCommand<Unit, IResolveResult<T>> Resolve { get; }

    bool CanResolve { get; }


    bool IsResolving { get; }

}