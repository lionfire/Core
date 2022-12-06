#nullable enable
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.Persistence.Handles;

public abstract class HandleProviderBase<TReference>
    where TReference : IReference
{
    public IReadHandle<T>? GetReadHandle<T>(IReference reference, T? preresolvedValue = default)
        => (reference is TReference fileReference) ? GetReadHandle<T>(fileReference) : null;

    public abstract IReadHandle<TValue> GetReadHandle<TValue>(TReference reference);
}
