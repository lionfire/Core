using System.Threading.Tasks;

namespace LionFire.Resolvables
{
    
    public interface IResolvesAsync
    {
        Task<IResolveResult> ResolveAsync();
    }
}
