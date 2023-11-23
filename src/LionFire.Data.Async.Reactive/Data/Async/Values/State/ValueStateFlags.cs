namespace LionFire.Data.Async;


[Flags]
public enum ValueStateFlags
{
    None = 0,
    HasReadCacheValue = 1 << 0,
    HasStagedValue = 1 << 1,

    IsGetting = 1 << 8,
    Polling = 1 << 9,

    IsSetting = 1 << 16,
    IsDebouncingSet = 1 << 17,

    // IStatelessGetter

    // IGetter

    // ISetter
    // IStagedSetter

}

