using LionFire.Persistence;
using LionFire.Resolves;
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Settings
{
    public interface IUserLocalSettings<out T> : ILazilyResolves<T>
        where T : class
    {
    }
}
