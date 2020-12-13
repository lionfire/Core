using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Structures;
using System.Linq;

namespace LionFire.Collections
{

    public static class IHierarchicalExtensions
    {
        public static T QuerySubPath<T>(this IHierarchyOfKeyed<T> hierarchical, params string[] pathChunks)
            where T : class 
            => hierarchical.QuerySubPath<T>(pathChunks, 0);

        public static T QuerySubPath<T>(this IHierarchyOfKeyed<T> hierarchical, string[] pathChunks, int index = 0)
            where T : class
        {
            if (hierarchical is IHasPathCache<string, T> hpc) { return hpc.PathCache[LionPath.FromPathArray(pathChunks, absolute: false)]; }

            if (!hierarchical.Children.ContainsKey(pathChunks[index])) return default;
            var next = hierarchical.Children[pathChunks[index]];
            if (index == pathChunks.Length - 1)
            {
                return next;
            }
            if (next is IHierarchyOfKeyed<T> h)
            {
                return h.QuerySubPath(pathChunks, index + 1);
            }
            else
            {
                return default;
                //throw new ArgumentException($"Cannot traverse path beyond chunk {index} because child does not implement IHierarchical<T>");
            }
        }

        public static T GetSubPath<T>(this IHierarchyOfKeyedOnDemand<T> hierarchical, string[] pathChunks, int index = 0)
            where T : class
        {
            var lastIndex = pathChunks.Length - 1;

            if (index == lastIndex) { return (T)hierarchical; }

            T next = default;

            for (; index < lastIndex; index++)
            {
                IHierarchyOfKeyed<T> h = hierarchical;

                next = h.Children.ContainsKey(pathChunks[index])
                    ? h.Children[pathChunks[index]]
                    : ((h as IHierarchyOfKeyedOnDemand<T>)
                        ?? throw new ArgumentException($"Cannot traverse path beyond chunk {index} because child does not exist and does not implement IHierarchicalOnDemand<T>"))
                        .GetChild(pathChunks[index]);

            }

            return next;
        }

        public static T QuerySubPath<T>(this IHierarchyOfKeyed<T> hierarchical, string path) where T : class => hierarchical.QuerySubPath(LionPath.ToPathArray(path));


        public static T GetSubPath<T>(this IHierarchyOfKeyedOnDemand<T> hierarchical, string path) where T : class => hierarchical.GetSubPath(LionPath.ToPathArray(path));
    }
}
