using LionFire.DependencyInjection;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    // REVIEW - Deprecate?, Use InjectionContext instead, but then maybe use this to point to InjectionContext for a nicer syntax.  Maybe rename to Ambient
    public static class Defaults
    {
        public static T Get<T>()
            where T : class, new()
        {
            var result = TryGet<T>(createIfMissing: true);
            if (result == null)
            {
                throw new Exception($"Failed to get or create non-null instance of {typeof(T).Name}");
                // Should never be reached, but this could be a fallback if the GetService in TryGet fails to create
                //ManualSingleton<T>.Instance = result = new T();
            }
            return result;
        }


        public static T TryGet<T>(Func<T> defaultProvider = null, bool createIfMissing = false)
            where T : class
        {
            T result;

            result = InjectionContext.Current?.GetService<T>(createIfMissing: createIfMissing);
            if (result != null) return result; // This is the new approach.  REVIEW below this

            var inst = ManualSingleton<T>.Instance;
            if (inst != null) return inst;

            var sp = ManualSingleton<IServiceProvider>.Instance;
            result = (T)sp?.GetService(typeof(T));
            if (result != null) return result;

            if (defaultProvider != null)
            {
                result = defaultProvider();
            }

            return result;
        }

        // FUTURE: Make the Set/Get methods extensible.  Need to have type as a parameter?
        //public Func<object> GetMethod { get; set; } = () =>
        //    {
        //        var inst = ManualSingleton<T>.Instance;
        //        if (inst != null) return inst;

        //        var sp = ManualSingleton<IServiceProvider>.Instance;
        //        return (T)sp?.GetService(typeof(T));
        //    };

        //public Action<object> SetMethod { get; set; } = () =>
        //   {
        //       if (ManualSingleton<T>.Instance != null) throw new InvalidOperationException("Already set");
        //       ManualSingleton<T>.Instance = obj;
        //   };

        public static void Set<T>(T obj)
                        where T : class
        {
            if (ManualSingleton<T>.Instance != null) throw new InvalidOperationException("Already set");

            ManualSingleton<T>.Instance = obj;
        }
    }
}
