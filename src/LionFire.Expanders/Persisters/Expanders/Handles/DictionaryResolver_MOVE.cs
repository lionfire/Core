using LionFire.Resolvers;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;

namespace LionFire.Resolvers;

public class DictionaryResolver<TKey, TValue> : IResolverSync<TKey, TValue>
    where TKey : notnull
{
    public Dictionary<TKey, TValue> Dictionary { get; set; } = new();

    public IResolveResult<TValue> Resolve(TKey resolvable)
    {
        if (Dictionary.TryGetValue(resolvable, out var result))
        {
            return new ResolveResultSuccess<TValue>(result);
        }
        else
        {
            return ResolveResultNotResolved<TValue>.Instance;
        }
    }
}

// ENH
public class RankedDictionaryResolver<TKey, TValue> : IResolver<TKey, TValue>
    where TKey : notnull
{
    public Dictionary<TKey, SortedList<float, Type>> Dictionary { get; set; } = new();


    public ITask<IResolveResult<TValue>> Resolve(TKey resolvable)
    {
        throw new NotImplementedException();
    }
}