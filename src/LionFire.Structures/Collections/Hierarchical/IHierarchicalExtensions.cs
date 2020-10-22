using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Collections
{
    public static class IHierarchicalExtensions
    {
        public static T QuerySubPath<T>(this IHierarchical<T> hierarchical, string[] pathChunks, int index = 0)
            where T : IHierarchical<T>
        {
            if (!hierarchical.Object.ContainsKey(pathChunks[index])) return default;
            var next = hierarchical.Object[pathChunks[index]];
            return index == pathChunks.Length - 1 ? next : next.QuerySubPath(pathChunks, index + 1);
        }
        public static T GetSubPath<T>(this IHierarchicalOnDemand<T> hierarchical, string[] pathChunks, int index = 0)
            where T : IHierarchicalOnDemand<T>
        {
            var next = hierarchical.GetChild(pathChunks[index]);
            return index == pathChunks.Length - 1 ? next : next.GetSubPath(pathChunks, index + 1);
        }
    }
}
