#if false // REVIEW
using LionFire.Persistence;
using LionFire.Referencing.Persistence;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public interface IHasObjectRetrievalInfo<ObjectType>
        where ObjectType : class
    {
        Task<RetrieveResult<ObjectType>> TryRetrieveObjectWithInfo();
        RetrieveResult<ObjectType>? ResolveHandleResult { get; }
    }
}
#endif