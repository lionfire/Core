using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Extensions
{
    public static class DefaultExtensions
    {
        public static bool IsDefault(this object obj, PropertyInfo pi)
        {
            var val = pi.GetValue(obj);
            if (pi.PropertyType.GetTypeInfo().IsClass) { if (val == null) return true; }
            else if (Activator.CreateInstance(pi.PropertyType) == val) { return true; }
            return false;
        }
        public static bool IsDefault<T>(this T val)
        {
            if (typeof(T).GetTypeInfo().IsClass) { if (val == null) return true; }
            else if (Activator.CreateInstance<T>().Equals(val)) { return true; }
            return false;
        }
    }
}
