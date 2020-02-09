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

    //public static class IReadPersisterExtensions
    //{
    //    // TODO: Extension methods for IReferencable > IReference?
    //    Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable)=>
    //    Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable)=>

    //    Task<IRetrieveResult<IEnumerable<string>>> List(IReferencable<TReference> referencable, ListFilter? filter = null)=>

    //}
}
