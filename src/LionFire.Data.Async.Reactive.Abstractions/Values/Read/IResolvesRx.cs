using LionFire.Data.Async.Gets;

namespace LionFire.Data.Async.Reactive;

public interface IResolvesRx<T> : IResolves<T>, IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject
{
}
