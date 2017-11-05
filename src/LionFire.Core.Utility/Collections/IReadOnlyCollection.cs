#if OLD // TOREVIEW
using System.Collections.Generic;

namespace LionFire.Collections
{

    /// <summary>
    /// TODO: Figure out how to support .NET 4.5
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyCollection<T> : IEnumerable<T>
    {
        #region From ICollection

        int Count { get; }
        bool IsReadOnly { get; }
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);

        #endregion

        T[] ToArray();
    }

}

#endif