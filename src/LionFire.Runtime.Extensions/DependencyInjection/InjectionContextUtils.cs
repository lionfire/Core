using LionFire.MultiTyping;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.DependencyInjection
{
    public static class InjectionContextUtils
    {
        private static char PathSeparator = '/';

        public static T AsTypeInPathOrDefault<T>(this string path, InjectionContext context = null)
            where T : class
        {
            return AsTypeInPath<T>(path, context = null) ?? ManualSingleton<T>.GuaranteedInstance;
        }

        public static T AsTypeInPath<T>(this string path, InjectionContext context = null)
            where T : class
        {
            var pathChunks = path.Split('/');

            if (context == null) context = InjectionContext.Current;

            T result;
            while (true)
            {
                result = ((MultiType)context.GetService(typeof(MultiType))).AsType<T>(path);

                if (result != null) return result;

                if (!path.Contains(PathSeparator)) break;
                path = path.Substring(0, path.LastIndexOf(PathSeparator));
            }
            return default(T);
        }
    }
}
