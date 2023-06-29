
namespace LionFire.Data;

public class AsyncSetOptions : AsyncGetOrSetOptions
{
    public bool BlockToSet { get; set; } = false;
    public SetTriggerMode SetTriggerMode { get; set; }
}
