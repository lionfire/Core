namespace LionFire.Results
{
    public struct NoopSuccessResult : ISuccessResult, INoopResult // REVIEW - there has got to be a better way to do this.  How performant is this?
    {
        public bool? IsSuccess => true;
        public bool IsNoop => true;
        public static readonly ISuccessResult Instance = new NoopSuccessResult();
    }
}
