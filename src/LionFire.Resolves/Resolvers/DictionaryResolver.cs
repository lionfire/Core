using LionFire.Data;
using LionFire.Data.Async.Gets;
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
            return new SuccessGetResult<TValue>(result);
        }
        else
        {
            return NotFoundGetResult<TValue>.Instance;
        }
    }
}
