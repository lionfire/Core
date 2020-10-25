using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Structures;

namespace LionFire.Collections
{
    public static class IHierarchicalExtensions
    {
        public static T QuerySubPath<T>(this IHierarchical<T> hierarchical, string[] pathChunks, int index = 0)
        {
            if (!hierarchical.Object.ContainsKey(pathChunks[index])) return default;
            var next = hierarchical.Object[pathChunks[index]];
            if (index == pathChunks.Length - 1)
            {
                return next;
            }
            if (next is IHierarchical<T> h)
            {
                return h.QuerySubPath(pathChunks, index + 1);
            }
            else
            {
                return default;
                //throw new ArgumentException($"Cannot traverse path beyond chunk {index} because child does not implement IHierarchical<T>");
            }
        }

        public static T GetSubPath<T>(this IHierarchicalOnDemand<T> hierarchical, string[] pathChunks, int index = 0)
            where T : class
        {
            var lastIndex = pathChunks.Length - 1;

            if(index == lastIndex) { return (T)hierarchical; }

            T next = default;

            for (; index < lastIndex;index++)
            {
                IHierarchical<T> h = hierarchical;

                next = h.Object.ContainsKey(pathChunks[index]) 
                    ? h.Object[pathChunks[index]]
                    : ((h as IHierarchicalOnDemand<T>) 
                        ?? throw new ArgumentException($"Cannot traverse path beyond chunk {index} because child does not exist and does not implement IHierarchicalOnDemand<T>"))
                        .GetChild(pathChunks[index]);

            }

            return next;
        }

        public static T QuerySubPath<T>(this IHierarchical<T> hierarchical, string path) => hierarchical.QuerySubPath(LionPath.GetChunks(path));


        public static T GetSubPath<T>(this IHierarchicalOnDemand<T> hierarchical, string path) where T : class => hierarchical.GetSubPath(LionPath.GetChunks(path));
    }
}
