using LionFire.Referencing.Resolution;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public interface IHasObjectRetrievalInfo<ObjectType>
        where ObjectType : class
    {
        Task<ResolveHandleResult<ObjectType>> TryResolveObjectWithInfo();
        ResolveHandleResult<ObjectType>? ResolveHandleResult { get; }
    }
}
