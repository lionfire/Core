using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Extensions.Rpc
{
    public static class ProxyUtils
    {
        public static List<Type> ProxyTypes = new List<Type>();

        public static void RegisterProxyType(Type type)
        {
            ProxyTypes.Add(type);
        }
    }

    public static class LionRpcObjectExtensions // MOVE
    {
        public static bool IsProxy(this object obj)
        {
            if(obj == null) return false;
            Type objType = obj.GetType();
            foreach (Type type in ProxyUtils.ProxyTypes)
            {
                if (type.IsAssignableFrom(objType)) return true;
            }
            return false;
        }
    }
}
