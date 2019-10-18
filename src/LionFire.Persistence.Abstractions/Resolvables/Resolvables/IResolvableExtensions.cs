using System;

namespace LionFire.Resolvables
{
    public static class IResolvableExtensions
    {
        public static Func<object, bool?> DefaultHasValueResolver = o =>
        {
            if (o is IResolvesEx rex)
            {
                return rex.HasValue;
            }
            return null;
        };

        public static Func<object, bool?> HasValueResolver = DefaultHasValueResolver;

        public static bool HasValue(this IResolvable resolvable) => HasValueResolver(resolvable) == true;

        /// <summary>
        /// Always returns false  for IResolvable, unless the object is also IAsyncResolvable in which case the answer is deferred to IsResolutionExpensive(this IAsyncResolvable).
        /// </summary>
        public static bool IsResolutionExpensive(this IResolvable resolvable)
        {
            if (resolvable is IAsyncResolvable ar)
            {
                return ar.IsResolutionExpensive();
            }
            return false;
        }
    }
}
