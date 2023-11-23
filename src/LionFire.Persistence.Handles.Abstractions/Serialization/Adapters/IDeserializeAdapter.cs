using LionFire.Persistence;
using System.Threading.Tasks;

namespace LionFire.Serialization.Adapters;

#if UNUSED
public interface IDeserializeAdapter<TFrom, TTo>
{
    Task<IGetResult<TTo>> Get(IReadHandle<TFrom> source);
}
#endif