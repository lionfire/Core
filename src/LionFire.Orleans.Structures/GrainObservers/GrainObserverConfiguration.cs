namespace LionFire.Orleans_;

public static class GrainObserverConfiguration
{
    public static Func<TimeSpan, TimeSpan> GetRenewInterval { get; set; } = (TimeSpan timeout)
      => TimeSpan.FromSeconds(Math.Min(timeout.TotalSeconds * 0.9, Math.Max(5, timeout.TotalSeconds - 20)));
}
