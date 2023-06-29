
namespace LionFire.Data;

public class AsyncSetOptions 
{
    public bool BlockToSet { get; set; } = false;
    public SetTriggerMode SetTriggerMode { get; set; }
}

public static class AsyncSetOptions<TValue>
{
    public static AsyncSetOptions Default { get; set; } = new();
}

public static class AsyncSetOptions<TKey, TValue>
{
    public static AsyncSetOptions Default { get; set; } = new();
}
