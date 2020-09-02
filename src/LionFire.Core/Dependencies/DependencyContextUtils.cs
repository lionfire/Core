using LionFire.MultiTyping;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Dependencies
{
    public static class DependencyContextUtils
    {
        private static readonly char PathSeparator = '/';

        public static T AsTypeInPathOrDefault<T>(this string path, DependencyContext context = null)
            where T : class
        {
            return AsTypeInPath<T>(path, context = null) ?? ManualSingleton<T>.GuaranteedInstance;
        }

        /// <summary>
        /// Get the service of type T that has previously been registered with the identifier of 'path'.
        /// </summary>
        /// <remarks>
        /// Registrations are stored within GetService(typeof(MultiType).AsType&lt;T&gt;(path)
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T AsTypeInPath<T>(this string path, DependencyContext context = null)
            where T : class
        {
            context ??= DependencyContext.Current;

            T result;
            while (true)
            {
                result = ((MultiType)context.GetService(typeof(MultiType))).AsType<T>(path);

                if (result != null) return result;

                if (!path.Contains(PathSeparator)) break;
                path = path.Substring(0, path.LastIndexOf(PathSeparator)); // MICROOPTIMIZE - split this one time
            }
            return default;
        }
    }
}
