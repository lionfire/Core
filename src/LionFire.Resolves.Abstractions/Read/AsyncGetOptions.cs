
namespace LionFire.Data;

/// <summary>
/// Options for properties (and potentially fields) accessible in an async manner
/// </summary>
public class AsyncGetOptions 
{
    //public bool TargetNotifiesPropertyChanged { get; set; }

    public bool ThrowOnGetValueIfHasValueIsFalse { get; set; } = false;
    public bool AutoGet { get; set; } // Get on init/ctor
    public TimeSpan AutoGetDelay { get; set; } = TimeSpan.FromMilliseconds(80);
    public int AutoGetDelayStaggerMilliseconds { get; set; } = 100;

    public bool GetOnDemand { get; set; } = true; // NOTIMPLEMENTED TODO REVIEW: there needs to be Task<IGetResult<T>> GetResult for this to be implemented

    /// <summary>
    /// For Sync Value property getter: block on the Task (not recommended, unless you are prepared for the drawbacks)
    /// </summary>
    public bool BlockToGet { get; set; } = false;

    /// <summary>
    /// Try to Dispose the cached Value when the object is disposed
    /// </summary>
    public bool DisposeValue { get; set; } = true;
}

public static class AsyncGetOptions<TValue> 
{
    public static AsyncGetOptions Default { get; set; } = new();
    
    public static IEqualityComparer<TValue> DefaultEqualityComparer { get; set; } = EqualityComparer<TValue>.Default;

}
public static class AsyncGetOptions<TKey, TValue>
{
    public static AsyncGetOptions Default { get; set; } = new();
}