using LionFire.Resolves;
using MorseCode.ITask;

namespace LionFire.Resolvers
{
    public interface IResolver<in TInput, out TOutput>
    {
        ITask<IResolveResult<TOutput>> Resolve(TInput resolvable);
    }
}
