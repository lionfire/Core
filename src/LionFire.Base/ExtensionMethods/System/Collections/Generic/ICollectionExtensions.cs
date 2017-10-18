using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods
{
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Sets me to match other
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="me"></param>
        /// <param name="other"></param>
        public static void SetToMatch<T>(this ICollection<T> me, IEnumerable<T> other, Func<T, bool> filter = null)
        {
            var otherArr = other.ToList();

            var meArr = me.ToList();

            if (filter == null) filter = _ => true;

            foreach (var item in otherArr.Where(filter))
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
