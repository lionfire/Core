#nullable enable

namespace LionFire.Mvvm;

public class AsyncPropertyOptions
{
    public bool AutoGetAll { get; set; }
    public bool AutoGetProperties { get; set; }

    public bool TargetNotifiesPropertyChanged { get; set; }
    public bool ThrowOnGetValueIfNotLoaded { get; set; } = false;
    public bool OptimisticGetWhileSetting { get; set; } = true;
    public bool GetOnDemand { get; set; } = true;
    public bool BlockToGet { get; set; } = false;
    public bool BlockToSet { get; set; } = false;
}
