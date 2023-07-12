#nullable enable
using LionFire.Data;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters;

/// <summary>
/// Partially pre-populate references used by this persister.  Can be used to provide a sort of sandbox or chroot environment
/// TOTEST
/// (TOSECURITY: verify users can't break out of it.)
/// </summary>
/// <typeparam name="TReference"></typeparam>
/// <typeparam name="TOverlayableReference"></typeparam>
public class CurriedPersister<TReference, TOverlayableReference> : IPersister<TReference>
    where TReference : IReference
{
    public IOverlayableReference<TReference> BaseReference { get; }

    public IPersister<TReference> Persister { get; }

    public ISerializationProvider? SerializationProvider => (Persister as ISerializingPersister)?.SerializationProvider;

    public CurriedPersister(IPersister<TReference> persister, IOverlayableReference<TReference> baseReference)
    {
        this.Persister = persister;
        this.BaseReference = baseReference;
    }

    public Task<ITransferResult> Exists<TValue>(IReferencable<TReference> referencable) => Persister.Exists<TValue>(BaseReference.AddRight(referencable.Reference));
    public Task<IGetResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable, RetrieveOptions? options = null) => Persister.Retrieve<TValue>(BaseReference.AddRight(referencable.Reference), options);
    public Task<ITransferResult> Create<TValue>(IReferencable<TReference> referencable, TValue value) => Persister.Create(BaseReference.AddRight(referencable.Reference), value);
    public Task<ITransferResult> Update<TValue>(IReferencable<TReference> referencable, TValue value) => Persister.Update(BaseReference.AddRight(referencable.Reference), value);
    public Task<ITransferResult> Upsert<TValue>(IReferencable<TReference> referencable, TValue value) => Persister.Upsert(BaseReference.AddRight(referencable.Reference), value);
    public Task<ITransferResult> Delete(IReferencable<TReference> referencable) => Persister.Delete(BaseReference.AddRight(referencable.Reference));
    public Task<IGetResult<IEnumerable<IListing<T>>>> List<T>(IReferencable<TReference> referencable, ListFilter? filter = null) => Persister.List<T>(referencable, filter);
}
