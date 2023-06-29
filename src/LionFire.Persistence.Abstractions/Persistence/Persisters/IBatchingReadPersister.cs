#nullable enable
using LionFire.Referencing;

namespace LionFire.Persistence.Persisters;

public interface IBatchingReadPersister<in TReference>
    where TReference : IReference
{
    IAsyncEnumerable<IGetResult<TValue>> RetrieveBatches<TValue>(IReferencable<TReference> referencable, RetrieveOptions? options = null);
}
