using System.Threading.Tasks;

namespace LionFire.Resolvables
{
    public interface IAsyncResolver<in TInput, TOutput>
    {
        //Task<TOutput> ResolveAsync<T>(T r) where T : TInput;
        Task<TOutput> ResolveAsync(TInput input);
    }
}
