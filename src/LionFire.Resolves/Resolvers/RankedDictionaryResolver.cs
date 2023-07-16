using LionFire.Data;

namespace LionFire.Resolvers;

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