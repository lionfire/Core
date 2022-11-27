#nullable enable
using LionFire.Persistence.TypeInference;
using LionFire.Referencing;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters;

public interface IReadPersister<in TReference>
    where TReference : IReference
{
    Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable);
    Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable);
}


//public static class IReadPersisterExtensions
//{
//    // TODO: Extension methods for IReferencable > IReference?
//    Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable)=>
//    Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable)=>

//    Task<IRetrieveResult<IEnumerable<string>>> List(IReferencable<TReference> referencable, ListFilter? filter = null)=>

//}
