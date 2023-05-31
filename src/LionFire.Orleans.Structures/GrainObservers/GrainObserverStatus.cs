namespace LionFire.Orleans_;

public class GrainObserverStatus
{
    public DateTimeOffset? LastRenewal { get; set; }
    public TimeSpan RenewInterval { get; set; }
    public TimeSpan? Timeout { get; set; }
}