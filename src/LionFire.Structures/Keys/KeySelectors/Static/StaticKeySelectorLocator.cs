#nullable enable

namespace LionFire.Structures;

/// <summary>
/// Returns most appropriate StaticKeySelector, as set in static variables.
/// 
/// Configure via StaticKeySelector<TItem, TKey>.Selector = (item) => item.Key;
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TKey"></typeparam>
public static class StaticKeySelectorLocator<TItem, TKey>
{
    // ENH OPTIMIZE: Do this at compile time in source generator

    static Func<TItem, TKey>? keySelector;

    public static bool TryGetKeySelector(out Func<TItem, TKey>? keySelector)
    {
        var x = TryGetKeySelector();
        if (x.success) { keySelector = x.keySelector; return true; }
        else { keySelector = null; return false; }
    }
    public static (bool success, Func<TItem, TKey>? keySelector) TryGetKeySelector()
    {
        if (keySelector != null) return (true, keySelector);

        keySelector = StaticKeySelector<TItem, TKey>.Selector;
        if (keySelector != null) return (true, keySelector);

        var genericType = typeof(StaticKeySelector<,>);

        HashSet<Type> seenInterfaces = new();

        for (var itemType = typeof(TItem); itemType != null; itemType = itemType == typeof(object) ? null : itemType.BaseType)
        {
            keySelector = genericType.MakeGenericType(itemType, typeof(TKey)).GetProperty(nameof(StaticKeySelector<TItem, TKey>.Selector))?.GetValue(null) as Func<TItem, TKey>;
            if (keySelector != null) return (true, keySelector);

            foreach (var interfaceType in itemType.GetInterfaces())
            {
                if (!seenInterfaces.Add(interfaceType)) continue;
                keySelector = genericType.MakeGenericType(interfaceType, typeof(TKey)).GetProperty(nameof(StaticKeySelector<TItem, TKey>.Selector))?.GetValue(null) as Func<TItem, TKey>;
                if (keySelector != null) return (true, keySelector);
            }
        }

        return (false, null);
    }
}
