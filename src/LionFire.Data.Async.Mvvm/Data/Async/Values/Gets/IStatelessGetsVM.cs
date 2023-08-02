using LionFire.Data.Gets;
using LionFire.Mvvm;
using System.Reactive;

namespace LionFire.Data.Gets.Mvvm;

public interface IGetsVMBase<T>
{
    ReactiveCommand<Unit, IGetResult<T>> GetCommand { get; }

    #region Derived

    bool CanGet { get; }

    bool IsGetting { get; }

    #endregion
}

public interface IStatelessGetsVM<T> 
    : IViewModel<IStatelessGets<T>> 
    , IGetsVMBase<T>
{
}
