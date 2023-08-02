
namespace LionFire.Data.Gets;

// ENH: make GetsSlim sealed for perf, and duplicate code here, with a tuple for ReadCacheValue and HasValue
public abstract class GetsDefaultableSlim<TValue> : GetterSlim<TValue>
{
    public override bool HasValue => hasValue;
    private bool hasValue;

    public override void DiscardValue()
    {
        base.DiscardValue();
        hasValue = false;
    }

    public override async ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        var resolveResult = await GetImpl(cancellationToken).ConfigureAwait(false);
        ReadCacheValue = resolveResult.Value;
        hasValue = true;
        return resolveResult;
    }
}

