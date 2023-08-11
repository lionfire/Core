namespace LionFire.Data.Mvvm;

using LionFire.Data.Async.Gets.Mvvm;
using LionFire.Data.Async.Sets.Mvvm;

public interface IValueVM<T>
    : //IViewModel<object>
     IGetterVMBase<T>
    , ISetsVMBase<T>
{
}

//public interface IValueVM<T>
//    : Reactive.IValueRx<T>
//{

//}