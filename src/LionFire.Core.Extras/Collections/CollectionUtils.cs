using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ExtensionMethods
{
    public static class CollectionUtils
    {
        public static void EnsureSize<T>(this ICollection<T> collection, int desiredCount, T emptyValue)
        {
            if (collection.Count < desiredCount)
            {
                for (int currentCount = collection.Count; currentCount < desiredCount; currentCount++)
                {
                    collection.Add(emptyValue);
                }
            }
        }

        public static void UpdateCollection<T>(this System.Collections.IList listToUpdate, IList<T> newList)
        {
            List<T> removals = new List<T>();

            foreach (var item in listToUpdate.OfType<T>())
            {
                if (!newList.Contains(item)) removals.Add(item);
            }

            foreach (var item in removals)
            {
                listToUpdate.Remove(item);
            }

            foreach (var item in newList)
            {
                if (!listToUpdate.Contains(item)) listToUpdate.Add(item);
            }
        }
    }
}
