namespace LionFire.Resolves;

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
