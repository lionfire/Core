#nullable enable
namespace LionFire.Results;

public interface IErrorResult
{
    object? Error { get; }
}

public static class IErrorResultX
{
    public static object? TryGetError(this IResult result) => (result as IErrorResult)?.Error;
}