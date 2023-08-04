
namespace LionFire.Data.Async.Sets;

public class SetterOptions : GetterOrSetterOptions
{
    public bool BlockToSet { get; set; } = false;
    public SetTriggerMode SetTriggerMode { get; set; }

    public bool RetainStagedValueAfterSet { get; set; }

    /// <summary>
    /// (TODO)
    /// After this duration, the residual state will be reset:
    ///   - StagedValue will me marked as Expired 
    ///   - SuccessfulSet will be unset
    ///   - FailedToSet will be unset
    /// </summary>
    public TimeSpan? Expiry { get; set; }

#if idea
    // ENH: Polly retry policy
    // TODO: init DoSet based on options?
    public Action<Func<Func<ITransferResult>>> DoSet { get; set; } = f => f()();
#endif

}
