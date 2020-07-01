using System.Collections;

namespace LionFire.ExtensionMethods
{
    public static class IEnumerableLinqExtensions
    {

        /// <summary>
        /// Supports null parameters
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool Any(this IEnumerable enumerable)
        {
            if (enumerable == null) return false;
            return ((IEnumerable)enumerable).GetEnumerator().MoveNext();
        }


        /// <summary>
        /// Supports null parameters
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool HasAtLeast(this IEnumerable enumerable, int count)
        {
            if (enumerable == null && count > 0) return false;

            var enumerator = enumerable.GetEnumerator();

            for (; count > 0; count--)
            {
                if (!enumerator.MoveNext()) return false;
            }
            return true;
        }
    }
}
