#nullable enable
using LionFire.Referencing;

namespace LionFire.Persistence.Handles;

public abstract class PreresolvableReadHandleProviderBase<TReference> : HandleProviderBase<TReference>
    , IPreresolvableReadHandleProvider<TReference>
    where TReference : IReference
{
    public IReadHandle<T>? GetReadHandle<T>(IReference reference)
    {
        throw new NotImplementedException();
    }

    public abstract IReadHandle<TValue> GetReadHandlePreresolved<TValue>(TReference reference, TValue preresolvedValue = default);

    public IReadHandle<T>? GetReadHandlePreresolved<T>(IReference reference, T preresolvedValue = default)
    {
        throw new NotImplementedException();
    }
}