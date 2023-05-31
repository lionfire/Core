
namespace LionFire.Mvvm;

public class AsyncPropertyOptions
{
    //public bool TargetNotifiesPropertyChanged { get; set; }
    public bool ThrowOnGetValueIfNotResolved { get; set; } = false;
    public bool OptimisticGetWhileSetting { get; set; } = true;
    public bool GetOnDemand { get; set; } = true;
    public bool BlockToGet { get; set; } = false;
    public bool BlockToSet { get; set; } = false;

    public SaveMode SaveMode { get; set; }
}

public class AsyncObjectOptions
{
    public bool AutoGet { get; set; }

}