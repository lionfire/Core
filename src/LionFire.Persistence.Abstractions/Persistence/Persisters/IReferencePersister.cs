#nullable enable
using LionFire.Referencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public interface IReferencePersister<in TReference>
            where TReference : IReference
    {
        Task<ITransferResult> Create<TValue>(TReference reference, TValue value);

        Task<ITransferResult> Exists<TValue>(TReference reference);

        Task<IRetrieveResult<TValue>> Retrieve<TValue>(TReference reference, RetrieveOptions? options = null);

        Task<ITransferResult> Update<TValue>(TReference reference, TValue value);
        Task<ITransferResult> Upsert<TValue>(TReference reference, TValue value);

        Task<ITransferResult> Delete(TReference reference);

        Task<IRetrieveResult<IEnumerable<IListing<T>>>> List<T>(TReference reference, ListFilter? filter = null);
    }
}
