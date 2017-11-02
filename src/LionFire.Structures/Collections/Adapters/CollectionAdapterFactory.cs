#define SourceToTargetMap_off
using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Instantiating;

namespace LionFire.Collections
{
    public static class CollectionAdapterFactory
    {
        public static IReadOnlyCollectionAdapter<TInstance> GetReadOnlyInstancesAdapter<TSource, TInstance>(this IEnumerable<TSource> sources)
            where TSource : class
        {
            var adapterType = GetCollectionAdapterType(sources, true);
            return (IReadOnlyCollectionAdapter<TInstance>)Activator.CreateInstance(adapterType, new object[] { sources });
        }

        public static ICollectionAdapter<TInstance> GetInstancesAdapter<TSource, TInstance>(this ICollection<TSource> sources)
            where TSource : class
        {
            var adapterType = GetCollectionAdapterType(sources);
            return (ICollectionAdapter<TInstance>)Activator.CreateInstance(adapterType, new object[] { sources });
        }

        private static Type GetCollectionAdapterType<TSource>(this IEnumerable<TSource> sources, bool readOnly = false)
            where TSource : class
        {
            var list = typeof(TSource).GetInterfaces().Where(i => i.FullName == typeof(IForInstancesOf<>).FullName).ToList();
            if (list.Count > 1) throw new NotSupportedException($"This method only supports a TSource that implements one type of {typeof(IForInstancesOf<>).FullName}");
            if (list.Count == 0) return null;
            var adapterType = (readOnly ? typeof(ReadOnlyCollectionAdapter<,>) : typeof(CollectionAdapter<,>)).MakeGenericType(list[0].GetGenericArguments()[0], typeof(TSource));
            return adapterType;
        }
    }
}
