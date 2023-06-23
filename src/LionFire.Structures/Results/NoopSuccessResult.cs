namespace LionFire.Results;

public struct NoopSuccessResult : ISuccessResult, INoopResult // REVIEW - there has got to be a better way to do this.  How performant is this?
{
    public static readonly ISuccessResult Instance = new NoopSuccessResult();

    public bool? IsSuccess => true;
    public bool IsNoop => true;
}
