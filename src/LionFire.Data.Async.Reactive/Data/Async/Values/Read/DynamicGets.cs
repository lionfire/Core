
namespace LionFire.Data.Gets;

public class DynamicGets<TKey, TValue> : AsyncGets<TKey, TValue>
    where TKey : class
    where TValue : class
{
    public DynamicGets(TKey key, AsyncValueOptions? options = null) : base(key, options)
    {
    }

    public Func<TKey, CancellationToken, ITask<IGetResult<TValue>>>? Getter { get; set; }

    protected override ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default) => (Getter ?? throw new ArgumentNullException(nameof(Getter)))(Key, cancellationToken);
}

