#nullable enable
//using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Structures;

public static class KeySelectorX
{
    public static Func<TItem, TKey> GetKeySelector<TItem, TKey>(this IServiceProvider serviceProvider, bool preferServiceProvider = false)
    {
        if (preferServiceProvider)
        {
            var result = ((Func<TItem, TKey>)serviceProvider.GetService(typeof(Func<TItem, TKey>)));
//            var result = serviceProvider.GetService<Func<TItem, TKey>>();
            if (result != null) return result;
        }

        if (typeof(IKeyed<TKey>).IsAssignableFrom(typeof(TItem)))
        {
            return i => (((IKeyed<TKey>?)i) ?? throw new ArgumentNullException()).Key;
        }
        else
        {
            return ((Func<TItem, TKey>)serviceProvider.GetService(typeof(Func<TItem, TKey>))) ?? throw new ArgumentException($"{nameof(TItem)} must be assignable to {nameof(IKeyed<TKey>)} or there must be a {nameof(Func<TItem, TKey>)} available from {nameof(IServiceProvider)}");
        }
    }
}
