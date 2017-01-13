using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.MultiTyping
{
    public interface IContainsMultiTyped
    {
        MultiTypeContainer MultiTyped { get; }
    }

    public static class IContainsMultiTypedExtensions
    {
        public static T AsType<T>(this IContainsMultiTyped cmt)
            where T : class
        {
            return cmt.MultiTyped.AsType<T>();
        }

        public static T AsTypeOrInject<T>(this IContainsMultiTyped cmt, InjectionContext context = null)
            where T : class
        {
            var result = cmt.MultiTyped.AsType<T>();
            if (result == null)
            {
                result = (context ?? InjectionContext.Current).GetService<T>();
                cmt.MultiTyped.SetType<T>(result);
            }
            return result;
        }
        public static T AsTypeOrCreate<T>(this IContainsMultiTyped cmt, Func<T> factory = null)
            where T : class
        {
            var result = cmt.MultiTyped.AsType<T>();
            if (result == null)
            {
                if (factory != null)
                {
                    result = factory();
                }
                else
                {
                    result = Activator.CreateInstance<T>();
                }
                cmt.MultiTyped.SetType<T>(result);
            }
            return result;
        }


        public static void SetType<T>(this IContainsMultiTyped cmt, T obj)
            where T : class
        {
            cmt.MultiTyped.SetType<T>(obj);
        }

        //public static void AddType<T>(this IMultiTyped mt, T obj)
        //{
        //    var list = mt.AsTypeOrCreate<IEnumerable<T>>(() => new List<T>());
        //}
    }


}
