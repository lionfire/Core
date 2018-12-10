using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace LionFire.Extensions.DefaultValues
{
    /// <remarks>
    /// TODO:
    ///  - Cache member setters
    /// Further reading: 
    ///  - https://stackoverflow.com/a/24291927/208304
    ///  - https://stackoverflow.com/a/29266437/208304
    /// </remarks>
    public static class DefaultValueUtils
    {
        public static object GetDefaultValue(this PropertyInfo mi)
        {
            var attr = mi.GetCustomAttribute<DefaultValueAttribute>();
            if (attr != null) return attr.Value;

            if (mi.PropertyType.IsValueType)
            {
                return Activator.CreateInstance(mi.PropertyType);
            }
            else
            {
                return null; // Default for reference types
            }
        }

        public static object GetDefaultValue(this FieldInfo mi)
        {
            var attr = mi.GetCustomAttribute<DefaultValueAttribute>();
            if (attr != null) return attr.Value;

            if (mi.FieldType.IsValueType)
            {
                return Activator.CreateInstance(mi.FieldType);
            }
            else
            {
                return null; // Default for reference types
            }
        }

        /// <summary>
        /// Apply the default values set in DefaultValueAttributes to public and non-public properties and fields in this instance.
        /// ENH: specify binding flags.
        /// OPTIMIZE: Allow a Ienumerable/HashSet for excludedMembers?
        /// RENAME: Set Members to DefaultValues
        /// </summary>
        public static void ApplyDefaultValues<T>(this T instance, params string[] excludedMembers)
        {
            foreach (PropertyInfo mi in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (excludedMembers.Length > 0 && !excludedMembers.Contains(mi.Name)) continue;
                if (mi.GetIndexParameters().Length > 0) continue;
                var attr = mi.GetCustomAttribute<DefaultValueAttribute>();
                if (attr != null)
                {
                    mi.SetValue(instance, attr.Value, null);
                }
            }

            foreach (FieldInfo mi in typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (excludedMembers.Length > 0 && !excludedMembers.Contains(mi.Name)) continue;
                var attr = mi.GetCustomAttribute<DefaultValueAttribute>();
                if (attr != null)
                {
                    mi.SetValue(instance, attr.Value);
                }
            }
        }

        public static void ApplyDefaultValues(this Type T)
        {
            foreach (PropertyInfo mi in (T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (mi.GetIndexParameters().Length > 0) continue;
                var attr = mi.GetCustomAttribute<DefaultValueAttribute>();
                if (attr != null)
                {
                    mi.SetValue(null, attr.Value, null);
                }
            }

            foreach (FieldInfo mi in (T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                var attr = mi.GetCustomAttribute<DefaultValueAttribute>();
                if (attr != null)
                {
                    mi.SetValue(null, attr.Value);
                }
            }
        }

        #region IsDefaultValue

        public static bool IsDefaultValue(this MemberInfo memberInfo, object instance = null)
        {
            var pi = memberInfo as PropertyInfo;
            if (pi != null) return IsDefaultValue(pi, instance);

            var fi = memberInfo as PropertyInfo;
            if (fi != null) return IsDefaultValue(fi, instance);

            throw new ArgumentException("memberInfo must be PropertyInfo or FieldInfo");
        }

        public static bool IsDefaultValue(this PropertyInfo mi, object instance = null)
        {
            var def = mi.GetDefaultValue();
            var cur = mi.GetValue(instance, null);
            if (def == null) return cur == null;
            return def.Equals(cur);
        }
        public static bool IsDefaultValue(this FieldInfo mi, object instance = null)
        {
            var def = mi.GetDefaultValue();
            var cur = mi.GetValue(instance);
            if (def == null) return cur == null;
            return def.Equals(cur);
        }

        #endregion

    }
}
