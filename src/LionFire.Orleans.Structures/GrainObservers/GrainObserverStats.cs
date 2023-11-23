namespace LionFire.Orleans_;

// ENH FUTURE
public class GrainObserverStats
{
    public DateTimeOffset? LastMessage { get; set; }
    public TimeSpan? LastSubscribeDuration { get; set; }
}
