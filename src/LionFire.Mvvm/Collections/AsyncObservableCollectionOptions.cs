namespace LionFire.Mvvm;

public class AsyncObservableCollectionOptions
{
    /// <summary>
    /// Automatically retrieve upon initialization (fire and forget) and subscribe to events to try to keep in sync
    /// </summary>
    public bool AutoSync { get; set; }

    public bool AlwaysRetrieveOnEnableSync { get; set; }

    public bool AutoInstantiateCollection { get; set; }

    //public TimeSpan? PollingInterval { get; set; } // ENH idea, for fallback for unreliable events, or redudancy

    public static readonly AsyncObservableCollectionOptions Default = new()
    {
        AlwaysRetrieveOnEnableSync = true,
        AutoInstantiateCollection = true,
        AutoSync = true,
    };
}
