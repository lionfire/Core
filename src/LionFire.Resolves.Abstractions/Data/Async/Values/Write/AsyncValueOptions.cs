
namespace LionFire.Data.Async;

public class AsyncValueOptions : AsyncGetOptions
{
    public bool BlockToSet { get; set; } = false;
    public SetTriggerMode SetTriggerMode { get; set; }
    public bool OptimisticGetWhileSetting { get; set; } = true;

}

public static class AsyncValueOptions<TValue>
{
    public static AsyncValueOptions Default { get; set; } = new();
}
public static class AsyncValueOptions<TKey,TValue>
{
    public static AsyncValueOptions Default { get; set; } = new();
}