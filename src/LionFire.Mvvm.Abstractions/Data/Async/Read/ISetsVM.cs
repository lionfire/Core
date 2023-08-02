namespace LionFire.Data.Mvvm;

using LionFire.Data.Sets;
using LionFire.Mvvm;
using System.Reactive;

public interface ISetsVMBase<T>
{
    ReactiveCommand<Unit, ITransferResult> SetCommand { get; }

    #region Derived

    bool CanSet { get; }

    bool IsSetting { get; }

    #endregion
}


public interface ISetsVM<T>
    : IViewModel<ISets<T>>
    , ISetsVMBase<T>
{
}
