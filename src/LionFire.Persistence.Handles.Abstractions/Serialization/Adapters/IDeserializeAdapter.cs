using LionFire.Persistence;
using System.Threading.Tasks;

namespace LionFire.Serialization.Adapters;


public interface IDeserializeAdapter<TFrom, TTo>
{
    Task<RetrieveResult<TTo>> Retrieve(IReadHandle<TFrom> source);
}
