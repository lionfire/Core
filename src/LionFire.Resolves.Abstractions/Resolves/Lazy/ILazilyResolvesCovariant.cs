using LionFire.Ontology;
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets
{
    public interface ILazilyResolvesCovariant<out T> : ILazilyResolves, IReadWrapper<T>
    {
        Task<ILazyResolveResult<object /* T */>> GetValue();
    }
    
}
