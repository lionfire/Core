using LionFire.DependencyInjection;
using System.Collections.Generic;

namespace LionFire.Resolvables
{
    public static class IResolvableExtensions
    {
        public static TValue Resolve<TKey, TValue>(this TKey resolvable)
        {
            {
                var resolver = DependencyContext.Current.GetService<IResolver<TKey, TValue>>();
                if (resolver != null) return resolver.Resolve(resolvable);
            }

            foreach(var resolver in DependencyContext.Current.GetService<IEnumerable<IResolver<TKey, TValue>>>())
            {
                var result = resolver.Resolve(resolvable);
                if (result != default) return result;
            }

            return default;
        }
    }
}
