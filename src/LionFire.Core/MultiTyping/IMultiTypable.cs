using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Immutable;

namespace LionFire.MultiTyping
{
    public interface IMultiTypable
    {
        IMultiTyped MultiTyped { get; }
    }

    public class MultiTypeTypeCollection<T> : ConcurrentList<T>
        where T : class
    {
    }

    public static class IMultiTypableExtensions
    {
        public static T AsType<T>(this IMultiTypable cmt)
            where T : class
        {
            return cmt?.MultiTyped?.AsType<T>();
        }

        public static T[] AsTypes<T>(this IMultiTypable cmt)
            where T : class
        {
            var list = cmt.AsType<MultiTypeTypeCollection<T>>();

            if (list != null)
            {
                return list.ToArray();
            }

            var single = cmt.AsType<T>();

            if (single != null) return new T[] { single };

            return Array.Empty<T>();
        }

        public static T AsTypeOrInject<T>(this IMultiTypable cmt, DependencyContext context = null)
            where T : class
        {
            var result = cmt.MultiTyped.AsType<T>();
            if (result == null)
            {
                result = (context ?? DependencyContext.Current).GetService<T>();
                cmt.MultiTyped.SetType<T>(result);
            }
            return result;
        }
        public static T AsTypeOrCreate<T>(this IMultiTypable cmt, Func<T> factory = null)
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

        public static IMultiTypable AddType<T>(this IMultiTypable cmt, T obj)
            where T : class
        {
            // TODO - readonly interface inside IMultiType instead of using ConcurrentList as a type
            {
                var list = cmt.AsType<MultiTypeTypeCollection<T>>();

                if (list != null)
                {
                    list.Add(obj);
                    return cmt;
                }
            }

            // TODO: Lock  TOTHREADSAFE

            var existing = cmt.AsType<T>();
            if (existing == null)
            {
                cmt.SetType<T>(obj);
                return cmt;
            }

            {
                var list = new MultiTypeTypeCollection<T>
                {
                    existing,
                    obj
                };

                cmt.SetType(list);
            }
            return cmt;
        }

        public static void SetType<T>(this IMultiTypable cmt, T obj)
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
