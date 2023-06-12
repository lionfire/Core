using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    public interface IMultiTyped : IReadOnlyMultiTyped
    {
        new object? this[Type type] { get; set; }
        T AsTypeOrCreateDefault<T>(Type? slotType = null, Func<T>? valueFactory = null) where T : class;

        void AddType<T>(T obj, bool allowMultiple = false) where T : class;

        /// <summary>
        /// Replace a single type (if any) with the provided object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        void SetType<T>(T? obj) where T : class;

        bool IsEmpty { get; }

    }


    // TODO REVIEW - IMultiTypedEx may be the commonly used interface, so I don't want it to have Ex on it.  What to do?
    // Call the simple one Minimal?

    //public interface IMultiTypedEx : IMultiTyped
    //{
    //}
}
