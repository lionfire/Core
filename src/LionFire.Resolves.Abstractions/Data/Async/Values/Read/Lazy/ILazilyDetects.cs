namespace LionFire.Data.Async.Gets;

// Triage:
// Note: If the object has not been retrieved but the object has been set on the handle, this will still initiate
// a retrieve which if an object was found may result in the detection of a merge conflict sooner than would otherwise be the case (FUTURE).


public interface ILazilyDetects<out T> : ILazilyGets<T>, IDiscardable
{

    ITask<bool> TryGetExists(bool saveValueIfAvailable = true);
    bool QueryExists();

    void DiscardExists();

    #region With Value

    /// <summary>
    /// Retrieves whether the object exists at the source, asynchronously, and stores the result.
    /// Subsequent calls will return the stored result.
    /// May return the actual Value if available
    /// </summary>
    /// <param name="saveValueIfAvailable"></param>
    /// <returns></returns>
    ITask<ILazyGetResult<T>> TryGetExistsWithValue(bool saveValueIfAvailable = true)
        => this.TryGetValue();

    ILazyGetResult<T> QueryExistsWithValue()
        => this.QueryValue();

    #endregion


}
