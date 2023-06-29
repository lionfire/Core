namespace LionFire.Data;

public static class EqualityComparerOptions<TValue>
{
    public static IEqualityComparer<TValue> Default { get; set; } = EqualityComparer<TValue>.Default;
}

