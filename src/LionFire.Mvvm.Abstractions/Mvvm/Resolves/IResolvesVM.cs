#nullable enable

using LionFire.Resolves;
using System.Reactive;

namespace LionFire.Mvvm;

public interface IResolvesVM<T> : IResolves<T>, IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject
{
    new ReactiveCommand<Unit, IResolveResult<T>> Resolve { get; }

     bool CanResolve { get; }

    
     bool IsResolving { get; }

}


