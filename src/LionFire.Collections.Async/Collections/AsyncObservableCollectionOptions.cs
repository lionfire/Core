namespace LionFire.Collections.Async;

public class AsyncObservableCollectionOptions
{
    // REVIEW: AutoSync, AlwaysRetrieveOnEnableSync: more logical way to do these?

    /// <summary>
    /// Automatically retrieve upon initialization (fire and forget) and subscribe to events to try to keep in sync
    /// </summary>
    public bool AutoSync { get; set; }

    public bool AlwaysRetrieveOnEnableSync { get; set; } // DEPRECATE - replace with optional parameter SetSync(retrieve: false)

    public bool AutoInstantiateCollection { get; set; } // DEPRECATE this probably

    //public TimeSpan? PollingInterval { get; set; } // ENH idea, for fallback for unreliable events, or redudancy

    public bool TrackOperationsInProgress { get; set; }

    public static readonly AsyncObservableCollectionOptions Default = new()
    {
        AlwaysRetrieveOnEnableSync = true,
        AutoInstantiateCollection = true,
        AutoSync = true,
    };


    // ENH Idea: IsAuthoritative: This Cache is the sole gatekeeper of the model in the system
    // ENH Idea: IsOptimisticAuthoritativeWrite: This Cache is authoritative, and assume writes succeed immediately without waiting for async

}
