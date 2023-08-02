namespace LionFire.Data.Mvvm;

using LionFire.Data.Gets.Mvvm;
using LionFire.Data.Sets.Mvvm;

public interface IValueVM<T>
    : //IViewModel<object>
     IGetsVMBase<T>
    , ISetsVMBase<T>
{
}

//public interface IValueVM<T>
//    : Reactive.IValueRx<T>
//{

//}