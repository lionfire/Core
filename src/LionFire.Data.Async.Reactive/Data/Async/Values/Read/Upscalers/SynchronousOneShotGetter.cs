using System.Reactive.Linq;

namespace LionFire.Data.Async.Gets;

public class FuncSynchronousOneShotGetter<TValue> : SynchronousOneShotGetter<TValue>
{

    public FuncSynchronousOneShotGetter(Func<TValue> getValue)
    {
        this.getValue = getValue;
    }

    public override TValue GetValue() => getValue();
    private Func<TValue> getValue;
}


/// <summary>
/// Treated as a preresolved Getter, but the value is calculated synchronously when first accessed.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public abstract class SynchronousOneShotGetter<TValue> : IGetter<TValue>
{
    public TValue? ReadCacheValue
    {
        get
        {
            if (!hasValue) DoGet();
            return readCacheValue;
        }
    }
    public TValue? readCacheValue;

    public TValue? Value => ReadCacheValue;

    public bool HasValue => hasValue;
    private bool hasValue;

    public IObservable<ITask<IGetResult<TValue>>> GetOperations => Observable.Empty<ITask<IGetResult<TValue>>>();

    public void Discard()
    {
        DiscardValue();
    }

    public void DiscardValue()
    {
        hasValue = false;
        readCacheValue = default;
    }

    public abstract TValue GetValue();

    private void DoGet()
    {
        readCacheValue = GetValue();
        hasValue = true;
    }

    public ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
        => Task.FromResult<IGetResult<TValue>>(new GetResult<TValue>(readCacheValue, true)).AsITask();

    public ITask<IGetResult<TValue>> GetIfNeeded() 
        => Task.FromResult<IGetResult<TValue>>(new GetResult<TValue>(ReadCacheValue, true)).AsITask();

    public IGetResult<TValue> QueryGetResult()
        => new GetResult<TValue>(readCacheValue, HasValue);
}
