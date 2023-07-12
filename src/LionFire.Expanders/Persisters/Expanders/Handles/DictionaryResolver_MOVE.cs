using LionFire.Data.Gets;
using MorseCode.ITask;

namespace LionFire.Resolvers;

public class DictionaryResolver<TKey, TValue> : IGetsSync<TKey, TValue>
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


    public ITask<IGetResult<TValue>> Get(TKey resolvable, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}