using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods
{
    public static class ICollectionExtensions
    {
        public static void SetToMatch<T>(this ICollection<T> me, IEnumerable<T> other)
        {
            var otherArr = other.ToList();

            var meArr = me.ToList();

            foreach (var item in otherArr)
            {
                if (!me.Contains(item))
                {
                    me.Add(item);
                }
                meArr.Remove(item);
            }
            foreach (var item in meArr)
            {
                me.Remove(item);
            }
        }
    }
}
