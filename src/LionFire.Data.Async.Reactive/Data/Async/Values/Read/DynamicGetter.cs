
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

    public override string ToString()
    {
        if (HasValue)
        {
            return Value?.ToString() ?? "{null}";
        }
        if (IsGetting)
        {
            return "{loading}";
        }
        return "{not loaded}";
    }
}

