
namespace LionFire.Data;

public class AsyncValueOptions : AsyncGetOptions
{
    public bool BlockToSet { get; set; } = false;
    public SetTriggerMode SetTriggerMode { get; set; }
    public bool OptimisticGetWhileSetting { get; set; } = true;

}
