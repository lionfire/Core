using LionFire.Resolves;
using MorseCode.ITask;

namespace LionFire.Resolvers;

public interface IResolver<in TInput, out TOutput>
{
    ITask<IResolveResult<TOutput>> Resolve(TInput resolvable);
}

public interface IResolverSync<in TInput, out TOutput>
{
    IResolveResult<TOutput> Resolve(TInput resolvable);
}
