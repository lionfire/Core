using LionFire.Persistence;
using LionFire.Data.Async.Gets;
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Settings
{
    public interface IUserLocalSettings<out T> : IGetter<T>
        where T : class
    {
    }
}
