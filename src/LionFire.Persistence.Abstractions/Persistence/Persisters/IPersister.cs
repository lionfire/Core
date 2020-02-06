#nullable enable
using LionFire.Referencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{


    public interface IReadPersister<in TReference>
        where TReference : IReference
    {
        Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable);
        Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable);

        Task<IRetrieveResult<IEnumerable<string>>> List(IReferencable<TReference> referencable, ListFilter? filter = null);

        ///// <summary>
        ///// Retrieve a list of names of child items
        ///// </summary>
        ///// <param name="path"></param>
        ///// <param name="filter"></param>
        ///// <returns></returns>
        //Task<IEnumerable<string>> List(string path, ListFilter? filter = null);
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

}
