using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.MultiTyping
{
    // RECENTCHANGE: Commented out the OfType, and added the extension methods.  This probably breaks things.
    // REVIEW: Make OfType an extenson method that does AsType<IEnumerable<>>?
    public interface SReadOnlyMultiTyped
    {
#if !NoGenericMethods
        T AsType<T>() where T : class; // REVIEW - is the class constraint required?
        //IEnumerable<T> OfType<T>() where T : class;
#else
        //object AsType(Type T);
        //IEnumerable<object> OfType(Type T);
#endif
    }

    public static class SReadOnlyMultiTypedExtensions
    {

        public static IEnumerable<T> OfType<T>(this SReadOnlyMultiTyped mt) where T : class 
            => mt.AsType<IEnumerable<T>>();
        public static IEnumerable<object> OfType(this SReadOnlyMultiTyped mt, Type T)
            => mt.AsType<IEnumerable<object>>().Where(o => T.IsAssignableFrom(o.GetType()));
    }
}
