using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class IServiceProviderExtensions
    {
        //Redundant with Microsoft extension method
        //public static T GetService<T>(this IServiceProvider sp)
        //{
        //    return (T)sp.GetService(typeof(T));
        //}

        public static void TryWithService<T>(this IServiceProvider serviceProvider, Action<T> action)
        {
            var result = (T)serviceProvider.GetService(typeof(T));
            if (result != default)
            {
                action(result);
            }
        }
    }
}
