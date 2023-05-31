#nullable enable


namespace LionFire.Mvvm;

public interface ICreatesAsyncVM<T>
{
    ReactiveCommand<ActivationParameters, Task<T>> Create { get; }
    IEnumerable<Type>? CreatableTypes { get; }
}

