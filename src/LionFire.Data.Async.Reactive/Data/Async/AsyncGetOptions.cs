
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

    #endregion
}
