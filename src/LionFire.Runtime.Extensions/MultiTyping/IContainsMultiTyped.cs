using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
