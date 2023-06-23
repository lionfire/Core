
namespace LionFire.Data;

/// <summary>
/// Options for properties (and potentially fields) accessible in an async manner
/// </summary>
public class AsyncGetOptions
{
    //public bool TargetNotifiesPropertyChanged { get; set; }

    #region Get

    public bool ThrowOnGetValueIfNotResolved { get; set; } = false;
    public bool AutoGet { get; set; }
    public bool GetOnDemand { get; set; } = true;
    public bool BlockToGet { get; set; } = false;

    /// <summary>
    /// Try to Dispose the cached Value when the object is disposed
    /// </summary>
    public static bool DisposeValue { get; set; } = true;

    #endregion
}
