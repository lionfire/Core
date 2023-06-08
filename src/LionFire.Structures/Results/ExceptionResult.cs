namespace LionFire.Results;

public struct ExceptionResult : ISuccessResult
{
    public ExceptionResult(Exception exception)
    {
        Exception = exception;
    }
    public bool? IsSuccess => false;

    public Exception Exception { get; }

}
