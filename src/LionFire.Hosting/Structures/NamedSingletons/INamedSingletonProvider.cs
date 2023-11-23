#nullable enable
using System.Threading.Tasks;

namespace LionFire.Structures
{
    public interface INamedSingletonProvider<TItem>
    {
        Task<TItem> Get(string key, params object[] parameters);
    }

}
