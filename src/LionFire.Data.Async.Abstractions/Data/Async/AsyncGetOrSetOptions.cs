
namespace LionFire.Data;

public class AsyncGetOrSetOptions
{
    /// <summary>
    /// Try to Dispose the cached Value when the object is disposed
    /// </summary>
    public bool DisposeValue { get; set; } = true;

    public bool DisposeKey { get; set; } = false;

    /// <summary>
    /// If false, return a Fail TransferResult with the Exception in the Error property.
    /// </summary>
    public bool ThrowOnException { get; set; } = true;

    
    // ENH: IObservable for errors when doing retries
    //ISubject<(object, Exception)>? Errors { get; set; }
}
