
namespace LionFire.Data;


public static class AsyncSetOptions<TValue>
{
    public static AsyncSetOptions Default { get; set; } = new();
}

public static class AsyncSetOptions<TKey, TValue>
{
    public static AsyncSetOptions Default { get; set; } = new();
}
