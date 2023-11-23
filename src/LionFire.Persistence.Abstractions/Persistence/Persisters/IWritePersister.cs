using LionFire.Referencing;
using LionFire.Serialization;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public interface IWritePersister<in TReference>
        where TReference : IReference
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="referencable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="AlreadySetException">When value already exists at referencable.Reference</exception>
        Task<ITransferResult> Create<TValue>(IReferencable<TReference> referencable, TValue value);

        /// <exception cref="NotFoundException">When existing value not found at referencable.Reference</exception>
        /// <returns></returns>
        Task<ITransferResult> Update<TValue>(IReferencable<TReference> referencable, TValue value);
        Task<ITransferResult> Upsert<TValue>(IReferencable<TReference> referencable, TValue value);

        // REVIEW - is this the only method that does not take a generic <TValue>?  Should it?
        Task<ITransferResult> DeleteReferencable(IReferencable<TReference> referencable);
    }

    //public interface IWritePersistenceOperationProvider<in TReference>
    //    where TReference : IReference
    //{
    //    Task<PersistenceOperation> MakeCreate<TValue>(IReferencable<TReference> referencable, TValue value);

    //    Task<PersistenceOperation> MakeUpdate<TValue>(IReferencable<TReference> referencable, TValue value);
    //    Task<PersistenceOperation> MakeUpsert<TValue>(IReferencable<TReference> referencable, TValue value);

    //    Task<PersistenceOperation> MakeDelete(IReferencable<TReference> referencable);
    //}
}
