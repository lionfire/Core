using LionFire.Persistence;
using LionFire.Data.Async.Gets;
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Settings
{
    public interface IUserLocalSettings<out T> : ILazilyGets<T>
        where T : class
    {
    }
}
