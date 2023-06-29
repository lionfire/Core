#nullable enable
using LionFire.Referencing;
using LionFire.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters;

public class PersisterFromReferencePersister<TReference> : IPersister<TReference>
    where TReference : IReference
{
    public IReferencePersister<TReference> Persister { get; }

    public ISerializationProvider? SerializationProvider { get; set; }

    public PersisterFromReferencePersister(IReferencePersister<TReference> persister)
    {
        Persister = persister;
    }

    public Task<ITransferResult> Create<TValue>(IReferencable<TReference> referencable, TValue value)
        => Persister.Create(referencable.Reference, value);

    public Task<ITransferResult> Exists<TValue>(IReferencable<TReference> referencable)
        => Persister.Exists<TValue>(referencable.Reference);

    public Task<IGetResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable, RetrieveOptions? options = null)
        => Persister.Retrieve<TValue>(referencable.Reference, options);

    public Task<ITransferResult> Update<TValue>(IReferencable<TReference> referencable, TValue value)
        => Persister.Update(referencable.Reference, value);

    public Task<ITransferResult> Upsert<TValue>(IReferencable<TReference> referencable, TValue value)
        => Persister.Upsert(referencable.Reference, value);

    public Task<ITransferResult> Delete(IReferencable<TReference> referencable)
        => Persister.Delete(referencable.Reference);

    public Task<IGetResult<IEnumerable<IListing<T>>>> List<T>(IReferencable<TReference> referencable, ListFilter? filter = null)
        => Persister.List<T>(referencable.Reference, filter);
    //public Task<IEnumerable<string>> List(string path, ListFilter? filter = null) => Persister.List(path, filter);
}
