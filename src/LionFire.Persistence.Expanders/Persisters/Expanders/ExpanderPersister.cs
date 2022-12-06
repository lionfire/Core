using LionFire.Persistence;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Persisters.Expanders;

public abstract class ExpanderPersister : PersisterBase<ExpanderOptions>, IExpander
{
    public abstract Task<RetrieveResult<TValue>> Retrieve<TValue>(ExpanderReadHandle<TValue> readHandle);
    public abstract Type? SourceReadType();
    public abstract Type? SourceReadTypeForReference(IReference reference);
    public abstract IReadHandle? TryGetReadHandle(IReference sourceReference);
}

