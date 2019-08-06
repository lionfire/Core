namespace LionFire.Persistence
{
    /// <summary>
    /// See also: IRetrieveHandleResult, which uses a (caching/shareable) handle instead.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <remarks>Doesn't use covariance because Tasks don't support covariance yet.</remarks>
    public interface IRetrieveResult<TObject> : IReadResult
    {
        TObject Object { get; }
    }

}