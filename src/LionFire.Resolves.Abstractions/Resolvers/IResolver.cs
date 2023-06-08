using LionFire.Data.Async.Gets;
using MorseCode.ITask;

namespace LionFire.Resolvers;

public interface IResolver<in TInput, out TOutput>
{
    ITask<IGetResult<TOutput>> Resolve(TInput resolvable);
}

public interface IResolverSync<in TInput, out TOutput>
{
    IGetResult<TOutput> Resolve(TInput resolvable);
}
