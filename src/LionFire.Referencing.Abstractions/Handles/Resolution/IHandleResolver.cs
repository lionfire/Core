using System.Threading.Tasks;

namespace LionFire.Referencing.Resolution
{
    public interface IHandleResolver
    {
        Task<ResolveHandleResult<T>> Resolve<T>(IReadHandle<T> handle)
            where T : class;
    }
}
