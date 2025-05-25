#nullable enable
using LionFire.Persistence.TypeInference;
using LionFire.Referencing;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters;

[Flags]
public enum RetrieveFlags : byte
{
    None = 0,
    //NotAmbiguous = 1 << 0,
    FirstSuccess = 1 << 1,

}

public class RetrieveOptions
{
    public static RetrieveOptions Default { get; } = new();

    public RetrieveFlags ValidationFlags { get; set; }

    /// <summary>
    /// Only return the first item found
    /// </summary>
    public bool ReturnFirstFound => ValidationFlags.HasFlag(RetrieveFlags.FirstSuccess);

}

public interface IReadPersister<in TReference>
    where TReference : IReference
{
    Task<ITransferResult> Exists<TValue>(IReferenceable<TReference> referencable);
    Task<IGetResult<TValue>> Retrieve<TValue>(IReferenceable<TReference> referencable, RetrieveOptions? options = null);
}


//public static class IReadPersisterExtensions
//{
//    // TODO: Extension methods for IReferenceable > IReference?
//    Task<ITransferResult> Exists<TValue>(IReferenceable<TReference> referencable)=>
//    Task<IGetResult<TValue>> Retrieve<TValue>(IReferenceable<TReference> referencable)=>

//    Task<IGetResult<IEnumerable<string>>> List(IReferenceable<TReference> referencable, ListFilter? filter = null)=>

//}
