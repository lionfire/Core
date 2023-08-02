namespace LionFire.Data.Mvvm;

using LionFire.Mvvm;

public interface IValueVM<T>
    : IViewModel<object>
    , IGetsVMBase<T>
    , ISetsVMBase<T>
{

}