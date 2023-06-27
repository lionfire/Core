using LionFire.Resolvers;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;
using LionFire.Data;

namespace LionFire.Resolvers;

public class DictionaryResolver<TKey, TValue> : IResolverSync<TKey, TValue>
    where TKey : notnull
{
    public Dictionary<TKey, TValue> Dictionary { get; set; } = new();

    public IGetResult<TValue> Resolve(TKey resolvable)
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
public class RankedDictionaryResolver<TKey, TValue> : IGetter<TKey, TValue>
    where TKey : notnull
{
    public Dictionary<TKey, SortedList<float, Type>> Dictionary { get; set; } = new();


    public ITask<IGetResult<TValue>> Resolve(TKey resolvable)
    {
        throw new NotImplementedException();
    }
}