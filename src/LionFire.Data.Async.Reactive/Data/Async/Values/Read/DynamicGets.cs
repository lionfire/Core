
namespace LionFire.Data.Async.Gets;

public class DynamicGets<TKey, TValue> : AsyncGets<TKey, TValue>
    where TKey : class
    where TValue : class
{
    public DynamicGets(TKey key, AsyncValueOptions? options = null) : base(key, options)
    {
    }

    public Func<TKey, ITask<IGetResult<TValue>>>? Getter { get; set; }

    protected override ITask<IGetResult<TValue>> GetImpl() => (Getter ?? throw new ArgumentNullException(nameof(Getter)))(Key);
}

