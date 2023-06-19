namespace LionFire.Results;

public interface ISuccessResult : IResult
{
    bool? IsSuccess { get; }
}
