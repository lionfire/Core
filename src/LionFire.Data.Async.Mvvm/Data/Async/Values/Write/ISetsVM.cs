namespace LionFire.Data.Async.Sets.Mvvm;

using LionFire.Mvvm;
using System.Reactive;

public interface ISetsVMBase<T>
{
    ReactiveCommand<Unit, ITransferResult> SetCommand { get; }// REVIEW - does this match impl?

    #region Derived

    bool CanSet { get; }

    bool IsSetting { get; }

    #endregion
}


public interface ISetsVM<T>
    : IViewModel<ISetter<T>>
    , ISetsVMBase<T>
{
}
