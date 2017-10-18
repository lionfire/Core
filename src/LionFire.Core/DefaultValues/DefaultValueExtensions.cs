using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Extensions
{
        // FUTURE: Support for DefaultValueAttribute
    public static class DefaultValueExtensions
    {

        //public static bool IsDefaultValueFromAttribute(this object obj, PropertyInfo pi)
        //{
        //    var val = pi.GetValue(obj);
        //    var attr = pi.GetCustomAttribute<DefaultValueAttribute>();
        //    if(attr == null) reutrn false;

        //    return pitattr.Value ==

        //    if (pi.PropertyType.GetTypeInfo().IsClass) { if (val == null) return true; }
        //    else if (Activator.CreateInstance(pi.PropertyType) == val) { return true; }
        //    return false;
        //}

        public static bool IsDefaultValue(this object obj, PropertyInfo pi)
        {
            var val = pi.GetValue(obj);
            if (pi.PropertyType.GetTypeInfo().IsClass) { if (val == null) return true; }
            else if (Activator.CreateInstance(pi.PropertyType) == val) { return true; }
            return false;
        }
        public static bool IsDefaultValue<T>(this T val)
        {
            if (typeof(T).GetTypeInfo().IsClass) { if (val == null) return true; }
            else if (Activator.CreateInstance<T>().Equals(val)) { return true; }
            return false;
        }
    }
}
