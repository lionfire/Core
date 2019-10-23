namespace LionFire.Results
{
    public struct FailResult : ISuccessResult
    {
        public bool? IsSuccess => false;
        public static readonly ISuccessResult Instance = new FailResult();
    }
}
