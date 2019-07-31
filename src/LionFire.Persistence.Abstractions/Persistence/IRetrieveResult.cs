namespace LionFire.Persistence
{
    /// <summary>
    /// See also: IRetrieveHandleResult, which uses a (caching/shareable) handle instead.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRetrieveResult<out T> : IReadResult
    {
        T Object { get; }
    }

}