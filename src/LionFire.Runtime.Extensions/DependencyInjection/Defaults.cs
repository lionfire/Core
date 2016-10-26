using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    public static class Defaults
    {
        public static T Get<T>()
            where T : class
        {
            var inst = ManualSingleton<T>.Instance;
            if (inst != null) return inst;

            var sp = ManualSingleton<IServiceProvider>.Instance;
            return (T)sp?.GetService(typeof(T));
        }

        public static void Set<T>(T obj)
            where T : class
        {
            if (ManualSingleton<T>.Instance != null) throw new InvalidOperationException("Already set");

            ManualSingleton<T>.Instance = obj;
        }
    }
}
