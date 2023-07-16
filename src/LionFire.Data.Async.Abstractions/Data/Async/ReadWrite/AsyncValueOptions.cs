
namespace LionFire.Data;

public class AsyncValueOptions
{
    public AsyncGetOptions Get { get; set; } = new();
    public AsyncSetOptions Set { get; set; } = new();
    public bool OptimisticGetWhileSetting { get; set; } = true;
}

public static class AsyncValueOptions<TValue>
{
    public static AsyncValueOptions Default { get; set; } = new();
}

public static class AsyncValueOptions<TKey, TValue>
{
    public static AsyncValueOptions Default { get; set; } = new();
}