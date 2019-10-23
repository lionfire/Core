namespace LionFire.Results
{
    public interface INoopResult : IResult
    {
        bool IsNoop { get; }
    }
}
