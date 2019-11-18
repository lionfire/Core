using System;
using LionFire.Collections.Concurrent;

namespace LionFire.Structures
{
    public static class ManualSingletonRegistrar
    {
        public static ConcurrentList<Type> List = new ConcurrentList<Type>();

        public static void ResetAll()
        {
            foreach (var type in List)
            {
                typeof(ManualSingleton<>).MakeGenericType(type).GetProperty("Instance").SetValue(null, null);
            }
        }
    }
}
