namespace LionFire.Data.Async.Gets;

public  struct ResolveResultSuccess<TValue> : IResolveResult<TValue>
{
    public ResolveResultSuccess(TValue value)
    {
        Value = value;
    }
    public bool? IsSuccess => true;

    public TValue Value { get; private set; }

    public bool HasValue => true;
}
