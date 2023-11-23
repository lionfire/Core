using ReactiveUI;

namespace LionFire.Reactive;

public interface IReactiveObjectEx
    : IReactiveNotifyPropertyChanged<IReactiveObject>
    , IHandleObservableErrors
    , IReactiveObject
{
}