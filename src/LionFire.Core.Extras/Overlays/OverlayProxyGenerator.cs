using System;
#if !NET35
using Castle.DynamicProxy;
#endif
using System.Reflection;

namespace LionFire.Overlays
{
#if !NET35
    internal class OverlayProxyGenerator : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
            //throw new NotImplementedException();
        }

        public void NonProxyableMemberNotification(Type type, System.Reflection.MemberInfo memberInfo)
        {
            //throw new NotImplementedException();
        }

        public bool ShouldInterceptMethod(Type type, System.Reflection.MethodInfo methodInfo)
        {
            if (
                methodInfo.Name.StartsWith("get_")
                || methodInfo.Name.StartsWith("set_"))
            {
                PropertyInfo pi = type.GetProperty(methodInfo.Name.Substring(4));
                if (pi.GetCustomAttribute<OverlayIgnoreAttribute>() != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }

#endif
}
