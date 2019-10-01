using System;
using System.Collections.Generic;

namespace LionFire.MultiTyping
{
    public interface SReadOnlyMultiTyped
    {
#if !NoGenericMethods
        T AsType<T>() where T : class;
        IEnumerable<T> OfType<T>() where T : class;
#endif
        object AsType(Type T);
        IEnumerable<object> OfType(Type T);
    }

}
