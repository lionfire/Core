#nullable enable


using LionFire.Reflection;

namespace LionFire.Data.Mvvm;

public interface ICreatesAsyncVM<T>
{
    ReactiveCommand<ActivationParameters, Task<T>> Create { get; }
    IEnumerable<Type> CreatableTypes { get; } // RENAME EffectiveCreatableTypes?  Or rename the conflicting name where it conflicts, to something else?  Or have both set and get methods on implementors, and remove the nullability on this, and always have a fallback to [].
}

