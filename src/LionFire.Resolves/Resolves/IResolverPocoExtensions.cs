using LionFire.Dependencies;
using LionFire.ExtensionMethods.Objects;
using LionFire.Ontology;
using LionFire.Resolvables;
using LionFire.Resolvers;
using LionFire.Resolves;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods.Poco.Resolvables
{
    public static class IResolverPocoExtensions
    {
        public static async Task<TValue> ResolveValue<TKey, TValue>(this TKey resolvable, bool returnFirstFound = true) => (await Resolve<TKey, TValue>(resolvable, returnFirstFound).ConfigureAwait(false)).Value;

        /// <summary>
        /// Resolve using a IResolver&lt;TKey, TValue&gt; service (or set of services) registered in DependencyContext.Current
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="resolvable"></param>
        /// <param name="returnFirstFound">If false, it will instead return first success (which may have HasValue == false)</param>
        /// <returns></returns>
        public static async Task<IResolveResult<TValue>> Resolve<TKey, TValue>(this TKey resolvable, bool returnFirstFound = true)
        {
            foreach (var resolver in resolvable.GetAmbientServiceProvider().GetServices<IResolver<TKey, TValue>>())
            {
                var result = await resolver.Resolve(resolvable).ConfigureAwait(false);
                if (returnFirstFound)
                {
                    if (result.HasValue) return result;
                }
                else
                {
                    if (result.IsSuccess == true) return result;
                }
            }
            return default;
        }
    }
}
