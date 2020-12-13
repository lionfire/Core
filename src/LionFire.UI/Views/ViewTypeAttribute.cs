#if NOESIS
using Noesis;
#else
#define Windowing
#endif
using System;

namespace LionFire.UI
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ViewTypeAttribute : Attribute
    {
        public Type Type { get; }

        public ViewTypeAttribute(Type type)
        {
            Type = type;
        }
    }


}
