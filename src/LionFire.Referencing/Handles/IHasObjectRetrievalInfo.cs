using LionFire.Referencing.Persistence;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public interface IHasObjectRetrievalInfo<ObjectType>
        where ObjectType : class
    {
        Task<RetrieveReferenceResult<ObjectType>> TryRetrieveObjectWithInfo();
        RetrieveReferenceResult<ObjectType>? ResolveHandleResult { get; }
    }
}
