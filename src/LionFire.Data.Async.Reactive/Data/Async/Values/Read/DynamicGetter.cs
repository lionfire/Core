
namespace LionFire.Data.Async.Gets;

public class DynamicGetter<TKey, TValue> : Getter<TKey, TValue>
    where TKey : class
    where TValue : class
{
    public DynamicGetter(TKey key, GetterOptions? options = null) : base(key, options)
    {
    }

    public Func<TKey, CancellationToken, ITask<IGetResult<TValue>>>? Getter { get; set; }

    protected override ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default) => (Getter ?? throw new ArgumentNullException(nameof(Getter)))(Key, cancellationToken);
}

