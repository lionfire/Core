#nullable enable
using LionFire.Referencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public interface IReferencePersister<in TReference>
            where TReference : IReference
    {
        Task<IPersistenceResult> Create<TValue>(TReference reference, TValue value);

        Task<IPersistenceResult> Exists<TValue>(TReference reference);

        Task<IRetrieveResult<TValue>> Retrieve<TValue>(TReference reference);

        Task<IPersistenceResult> Update<TValue>(TReference reference, TValue value);
        Task<IPersistenceResult> Upsert<TValue>(TReference reference, TValue value);

        Task<IPersistenceResult> Delete(TReference reference);

        Task<IRetrieveResult<IEnumerable<string>>> List(TReference reference, ListFilter? filter = null);
    }
}
