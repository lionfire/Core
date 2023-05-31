#nullable enable

namespace LionFire.Structures;

/// <summary>
/// Configure your selectors here for known TItem to TKey scenarios.
/// See also: 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TKey"></typeparam>
public static class StaticKeySelector<TItem, TKey>
{
    public static Func<TItem, TKey>? Selector { get; set; }
}
