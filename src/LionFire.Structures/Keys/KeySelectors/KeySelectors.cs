#nullable enable
//using Microsoft.Extensions.DependencyInjection;

using System;

namespace LionFire.Structures;


public static class KeySelectors<TItem, TKey>
{
    public static readonly Func<TItem, TKey>? Cast = i => i == null ? throw new ArgumentNullException() : (TKey)(object)i;
    public static readonly Func<TItem, TKey>? FromKeyed = i => (((IKeyed<TKey>?)i) ?? throw new ArgumentNullException()).Key;

    /// <summary>
    /// Try to obtain a Func for extracting TKey from TItem using these strategies:
    ///  - item => IKeyed<TKey>.Key
    ///  - StaticKeySelectorLocationStrategy<TItem, TKey>.Selector
    ///  
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static Func<TItem, TKey>? TryGetKeySelector(IServiceProvider? serviceProvider = null)
    {
        Func<TItem, TKey>? result = null;

        if (typeof(TKey).IsAssignableFrom(typeof(TItem))) { return Cast; }
        if (typeof(IKeyed<TKey>).IsAssignableFrom(typeof(TItem))) { return FromKeyed; }

        if (StaticKeySelectorLocator<TItem, TKey>.TryGetKeySelector(out result)) { return result!; }

        return ((Func<TItem, TKey>?)(serviceProvider)?.GetService(typeof(Func<TItem, TKey>)));
    }

    public static Func<TItem, TKey> GetKeySelector(IServiceProvider? serviceProvider = null)
    {
        return TryGetKeySelector(serviceProvider)
            ?? throw new ArgumentException(
                $"{nameof(TItem)} must be assignable to {nameof(IKeyed<TKey>)}, or have {nameof(StaticKeySelector<TItem, TKey>)}.{nameof(StaticKeySelector<TItem, TKey>.Selector)} set for it or a base class or interface of {nameof(TItem)}, or there must be a {nameof(Func<TItem, TKey>)} available from {nameof(IServiceProvider)}"
                );
    }
}
