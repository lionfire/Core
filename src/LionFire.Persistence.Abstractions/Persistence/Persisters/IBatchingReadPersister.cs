#nullable enable
using LionFire.Referencing;

namespace LionFire.Persistence.Persisters;

public interface IBatchingReadPersister<in TReference>
    where TReference : IReference
{
    IAsyncEnumerable<IRetrieveResult<TValue>> RetrieveBatches<TValue>(IReferencable<TReference> referencable, bool breakOnFirstValue = true);
}
