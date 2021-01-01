using System;
using System.Reflection;
#if OverlayProxies
using Castle.DynamicProxy;
#endif

namespace LionFire.Overlays
{
#if OverlayProxies
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

        public override bool Equals(object obj) => Object.ReferenceEquals(this, obj);
        public override int GetHashCode() => base.GetHashCode();
    }

#endif
}
