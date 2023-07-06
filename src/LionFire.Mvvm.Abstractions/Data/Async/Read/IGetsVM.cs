using LionFire.Data.Gets;
using LionFire.Mvvm;
using System.Reactive;

namespace LionFire.Data.Mvvm;

public interface IGetsVM<T> : IViewModel<IGets<T>>
{
    ReactiveCommand<Unit, IGetResult<T>> GetCommand { get; }

    #region Derived

    bool CanGet { get; }

    bool IsGetting { get; }

    #endregion

}