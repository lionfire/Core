using LionFire.Ontology;
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

public interface ILazilyGetsCovariant<out T> : ILazilyGets, IReadWrapper<T>
{
    Task<ILazyGetResult<object /* T */>> GetValue();
}

