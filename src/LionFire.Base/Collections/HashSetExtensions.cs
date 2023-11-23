namespace LionFire.ExtensionMethods.Collections;

public static class HashSetExtensions
{
    public static void TryAddRange<T>(this HashSet<T> hashSet, IEnumerable<T> values)
    {
        foreach(var value in values)
        {
            hashSet.TryAdd(value);
        }
    }
}
