#nullable enable
using LionFire.Persistence.TypeInference;
using LionFire.Referencing;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters;

[Flags]
public enum RetrieveValidateFlags : byte
{
    None = 0,
    NotAmbiguous = 1 << 0,
}

public class RetrieveOptions
{
    public static RetrieveOptions Default { get; } = new();

    public RetrieveValidateFlags ValidationFlags { get; set; }

    /// <summary>
    /// Only return the first item found
    /// </summary>
    public bool ReturnFirstFound => !ValidationFlags.HasFlag(RetrieveValidateFlags.NotAmbiguous);

}

public interface IReadPersister<in TReference>
    where TReference : IReference
{
    Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable);
    Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable, RetrieveOptions? options = null);
}


//public static class IReadPersisterExtensions
//{
//    // TODO: Extension methods for IReferencable > IReference?
//    Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable)=>
//    Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable)=>

//    Task<IRetrieveResult<IEnumerable<string>>> List(IReferencable<TReference> referencable, ListFilter? filter = null)=>

//}
