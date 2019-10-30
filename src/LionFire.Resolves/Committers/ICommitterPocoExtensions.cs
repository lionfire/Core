using LionFire.DependencyInjection;
using LionFire.Resolvables;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Resolves;
using LionFire.Resolvers;

namespace LionFire.ExtensionMethods.Poco.Resolvables
{
    public static class ICommitterPocoExtensions
    {
        /// <summary>
        /// Put using a ICommitter&lt;TKey, TValue&gt; service (or set of services) registered in DependencyContext.Current
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="returnFirstSuccess">If false (default), it will instead return first result that has IsSuccess().HasValue == true.  If true, it will try all ICommitters until one returns IsSuccess() == true.</param>
        /// <returns></returns>
        public static async Task<IPutResult> Commit<TKey, TValue>(this TKey key, TValue value, bool returnFirstSuccess = false)
        {
            foreach (var resolver in DependencyContext.Current.GetServices<ICommitter<TKey, TValue>>())
            {
                var result = await resolver.Commit(key).ConfigureAwait(false);
                if (!returnFirstSuccess)
                {
                    if (result.IsSuccess().HasValue) return result;
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
