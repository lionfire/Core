using System;

namespace LionFire.MultiTyping
{
    // REVIEW this

    public interface SReadOnlyMultiTypedEx
    {
#if !NoGenericMethods
        T AsType<T>() where T : class;
        T[] OfType<T>() where T : class;
#endif
        object AsType(Type T);
        object[] OfType(Type T);
    }

}
