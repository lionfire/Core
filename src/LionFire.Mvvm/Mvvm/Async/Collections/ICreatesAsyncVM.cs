#nullable enable


using LionFire.Reflection;

namespace LionFire.Data.Async.Mvvm;

public interface ICreatesAsyncVM<T>
{
    ReactiveCommand<ActivationParameters, Task<T>> Create { get; }
    IEnumerable<Type>? CreatableTypes { get; }
}

