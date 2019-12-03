using LionFire.Referencing;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    //public enum PersisterOptionFlags
    //{
    //    EtagsSupported = 1 << 1,
    //    EtagsRequired = 1 << 1,
    //}
    //public struct PersisterOptions
    //{
    //}

    //public enum PersistenceOperationType
    //{
    //    Unspecified = 0,
    //    Create = 1 << 0,
    //    Retrieve = 1 << 1,
    //    Exists = 1 << 2,
    //    Update = 1 << 3,
    //    Upsert = 1 << 4,
    //    Delete = 1 << 5,

    //    /// <summary>
    //    /// Retrieves last eTag
    //    /// </summary>
    //    IsChanged = 1 << 10,

    //    List = 1 << 16,
    //    Add = 1 << 17,
    //    Remove = 1 << 18,
    //}
    
    public interface IReadPersister<in TReference>
        where TReference : IReference
    {
        Task<IPersistenceResult> Exists(IReferencable<TReference> referencable);
        //where TReference : IReference;
        Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable);
        //where TReference : IReference;
    }


    //public interface IPersister : IPersister<IReference>
    //{
    //}
    public interface IPersister<in TReference> : IReadPersister<TReference>, IWritePersister<TReference>
        where TReference : IReference
    {
    }


    public interface ICollectionPersister
    {
        //    Task<IPersistenceResult> List<TReference, TValue>(IReferencable<TReference> reference, TValue value)
        //        where TReference : IReference;
        //    Task<IPersistenceResult> Add<TReference, TValue>(IReferencable<TReference> reference, TValue value)
        //where TReference : IReference;

        //    Task<IPersistenceResult> Remove<TReference, TValue>(IReferencable<TReference> reference, TValue value)
        //where TReference : IReference;

    }
}
