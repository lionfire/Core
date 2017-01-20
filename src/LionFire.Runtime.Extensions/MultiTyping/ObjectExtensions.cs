using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    public static class ObjectExtensions
    {
        // REVIEW - I don't think I want object extensions.  Did I need this for something?
        //public static T AsType<T>(this object obj)
        //    where T : class
        //{
        //    T result = obj as T;
        //    if (result != null) return result;

        //    var mt = obj as IMultiTyped;
        //    if (mt == null)
        //    {
        //        var cmt = obj as IContainsMultiTyped;
        //        mt = cmt?.MultiTyped;
        //    }
        //    if (mt == null) return null;

        //    return mt.AsType<T>();
        //}


        //public static T AsType<T>(this IContainsMultiTyped obj)
        //    where T : class
        //{
        //    var cmt = obj as IContainsMultiTyped;
        //    var mt = cmt?.MultiTyped;
        //    if (mt == null) return null;

        //    return mt.AsType<T>();
        //}

        #region AsTypeFromProperties


        public static T AsTypeFromProperties<T>(this object obj, Func<Type, PropertyInfo, bool> propertyFilter = null)
            where T : class
        {
            if (propertyFilter == null) propertyFilter = DefaultPropertyFilter;

            return (T)obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(_pi => typeof(T).IsAssignableFrom(_pi.PropertyType) && propertyFilter(typeof(T), _pi))
               .FirstOrDefault()
               ?.GetValue(obj);
        }

        public static bool DefaultPropertyFilter(Type type, PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name == type.Name) return true;
            //if (propertyInfo.Name.Length >= 2)
            //{
            //    if (propertyInfo.Name.StartsWith("I") && char.IsUpper(propertyInfo.Name[1]) && propertyInfo.Name.Substring(1) == type.Name) return true;
            //}
            return false;
        }

        #endregion


    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class MultiTypeFromProperties : Attribute
    {
    }
}
