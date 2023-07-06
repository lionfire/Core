
namespace LionFire.Data;

public class AsyncSetOptions : AsyncGetOrSetOptions
{
    public bool BlockToSet { get; set; } = false;
    public SetTriggerMode SetTriggerMode { get; set; }

#if idea
    // ENH: Polly retry policy
    // TODO: init DoSet based on options?
    public Action<Func<Func<ITransferResult>>> DoSet { get; set; } = f => f()();
#endif

}
