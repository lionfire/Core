using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    // Deprecated, Use InjectionContext instead, but then maybe use this to point to InjectionContext for a nicer syntax.  Maybe rename to Ambient
    public static class Defaults
    {
        public static T Get<T>()
            where T : class, new()
        {
            var result = TryGet<T>();
            if (result == null)
            {
                ManualSingleton<T>.Instance = result = new T();
            }
            return result;
        }
        public static T TryGet<T>()
            where T : class
        {
            var inst = ManualSingleton<T>.Instance;
            if (inst != null) return inst;

            var sp = ManualSingleton<IServiceProvider>.Instance;
            return (T)sp?.GetService(typeof(T));
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
