using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Ontology;
using LionFire.Structures;

namespace LionFire.Validation.MultiTyping
{
}
namespace LionFire.MultiTyping
{
    public static class ObjectMultiTypingExtensions
    {
        /// <summary>
        /// Checks for whether obj has the specified type and has had that type created.  The type will not be created on demand.  (To create it on demand, use ObjectAsType instead.)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ObjectHasType<T>(this object obj)
            where T : class
        {
            if (obj == null) return null;

            if (obj is T result) return result;

            if (obj is IHas<T> iis)
            {
                if (obj is IHasLateCreatable<T> lci)
                {
                    if (lci.HasObject && iis.Object != null) return iis.Object;
                }
                else
                {
                    T iisObj = iis.Object;
                    if (iisObj != null) return iisObj;
                }
            }

            var mt = obj as IMultiTyped;
            if (mt == null)
            {
                var cmt = obj as IMultiTypable;
                mt = cmt?.MultiTyped;
            }
            if (mt != null)
            {
                var mtChild = mt.AsType<T>();
                if (mtChild != null) return mtChild;
            }

            if (obj is IServiceProvider sp)
            {
                var service = (T)sp.GetService(typeof(T));
                if (service != null) return service;
            }

            return null;
        }

        /// <summary>
        /// Attempts to return obj as the specified type.  The type may be created on demand.  (To check for the existance of the type, use ObjectHasType instead.)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ObjectAsType<T>(this object obj)
            where T : class
        {
            if (obj == null) return null;

            if (obj is T result) return result;

            T isObject = ((obj as IHas<T>)?.Object);
            if (isObject != null) return isObject;

            var mt = obj as IMultiTyped;
            if (mt == null)
            {
                var cmt = obj as IMultiTypable;
                mt = cmt?.MultiTyped;
            }
            if (mt != null)
            {
                var mtChild = mt.AsType<T>();
                if (mtChild != null) return mtChild;
            }

            // DEPRECATE this?  Or is it a good tie-in to Microsoft interfaces even though it is different terminology (service provision)?
            if (obj is IServiceProvider sp)
            {
                var service = (T)sp.GetService(typeof(T));
                if (service != null) return service;
            }

            // TODO: Use DependencyContext parameter to do further querying
            // TODO: Use static DependencyContext.Current to do further querying

            return null;
        }


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
