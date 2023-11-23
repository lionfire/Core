#nullable enable
//using Microsoft.Extensions.DependencyInjection;

using System;

namespace LionFire.Structures;


public static class KeySelectors
{
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
    public static Func<TItem, TKey>? TryGetKeySelector<TItem, TKey>(IServiceProvider? serviceProvider = null)
    {
        Func<TItem, TKey>? result = null;

        if (typeof(IKeyed<TKey>).IsAssignableFrom(typeof(TItem)))
        {
            return i => (((IKeyed<TKey>?)i) ?? throw new ArgumentNullException()).Key;
        }

        if (StaticKeySelectorLocator<TItem, TKey>.TryGetKeySelector(out result)) { return result!; }

        return ((Func<TItem, TKey>?)(serviceProvider)?.GetService(typeof(Func<TItem, TKey>)));
    }

    public static Func<TItem, TKey> GetKeySelector<TItem, TKey>(IServiceProvider? serviceProvider = null)
    {
        return TryGetKeySelector<TItem, TKey>(serviceProvider)
            ?? throw new ArgumentException(
                $"{nameof(TItem)} must be assignable to {nameof(IKeyed<TKey>)}, or have {nameof(StaticKeySelector<TItem, TKey>)}.{nameof(StaticKeySelector<TItem, TKey>.Selector)} set for it or a base class or interface of {nameof(TItem)}, or there must be a {nameof(Func<TItem, TKey>)} available from {nameof(IServiceProvider)}"
                );
    }

}
