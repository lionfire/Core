
namespace LionFire.Data.Async;

public class ValueOptions
{
    public GetterOptions Get { get; set; } = new();
    public SetterOptions Set { get; set; } = new();
    public bool OptimisticGetWhileSetting { get; set; } = true;
}

public static class ValueOptions<TValue>
{
    public static ValueOptions Default { get; set; } = new();
}

public static class ValueOptions<TKey, TValue>
{
    public static ValueOptions Default { get; set; } = new();
}