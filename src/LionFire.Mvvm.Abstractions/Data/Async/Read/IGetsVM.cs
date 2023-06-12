using LionFire.Data.Async.Gets;
using LionFire.Mvvm;
using System.Reactive;

namespace LionFire.Data.Async.Mvvm;

public interface IGetsVM<T> : IViewModel<IGets<T>>
{
    ReactiveCommand<Unit, IGetResult<T>> GetCommand { get; }

    bool CanGet { get; }

    bool IsGetting { get; }

}