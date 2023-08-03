using LionFire.Data.Async.Gets;
using LionFire.Mvvm;
using System.Reactive;

namespace LionFire.Data.Async.Gets.Mvvm;

public interface IGetterVMBase<T>
{
    ReactiveCommand<Unit, IGetResult<T>> GetCommand { get; }

    #region Derived

    bool CanGet { get; }

    bool IsGetting { get; }

    #endregion
}

public interface IStatelessGetterVM<T> 
    : IViewModel<IStatelessGetter<T>> 
    , IGetterVMBase<T>
{
}
