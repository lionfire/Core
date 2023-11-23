
namespace LionFire.Data.Async.Sets;


public static class SetterOptions<TValue>
{
    public static SetterOptions Default { get; set; } = new();
}

public static class SetterOptions<TKey, TValue>
{
    public static SetterOptions Default { get; set; } = new();
}
