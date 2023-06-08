using LionFire.Persistence;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Data.Async.Gets;

namespace LionFire.Persisters.Expanders;

// REVIEW: Add TSourceValue? Or would it mess up expanders with multiple source types?
public abstract class ExpanderPersister : PersisterBase<ExpanderOptions>, IExpander
{
    public abstract Task<RetrieveResult<TValue>> Retrieve<TValue>(ExpanderReadHandle<TValue> readHandle);
    public abstract Type? SourceReadType();
    public abstract Type? SourceReadTypeForReference(IReference reference);

    public abstract Task<IReadHandle>? TryGetSourceReadHandle(IReference sourceReference);
    
    public abstract Task<IRetrieveResult<T>> RetrieveTarget<T>(IReadHandle sourceReadHandle, string targetPath);
}

