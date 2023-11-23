namespace LionFire.Data.Async;

[Flags]
public enum TriggerMode
{
    Unspecified = 0,

    Disabled = 1 << 0,

    Once = 1 << 1,

    Manual = 1 << 2,

    /// <summary>
    /// Get: use standard buffer/throttle/batch options
    /// 
    /// Set: trigger the setting of data with a reasonable trigger point, such as blur from a text field.
    /// </summary>
    Auto = 1 << 3,
    
    /// <summary>
    /// Get: unbuffered, unthrottled, unbatched get
    /// 
    /// Set: set data as soon as changes are available.  Example: user typing into a text field will have the data set after each key press.
    /// </summary>
    AutoImmediate = 1 << 4,
}
