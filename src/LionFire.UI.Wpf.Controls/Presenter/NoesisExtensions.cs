#if NOESIS
using Noesis;
#else
#define Windowing
#endif
using System;
using System.Reflection;
//using LionFire.Vos.VosApp;

namespace LionFire.ExtensionMethods
{
    public static class NoesisExtensions
    {
        public static T GetCustomAttribute2<T>(this MemberInfo type)
            where T : Attribute
            => System.Reflection.CustomAttributeExtensions.GetCustomAttribute<T>(type);
    }
}
