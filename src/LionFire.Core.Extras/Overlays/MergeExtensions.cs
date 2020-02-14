#define SanityChecks
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LionFire.Overlays;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace LionFire.Extensions.Merging
{
    public interface IMergeable
    {
        object MergeFrom(object source);
    }

    public enum MergeOptions
    {
        Unspecified = 0,
        Shallow = 1,
        Deep = 2,
        DeepForIMergeableOnly = 3,
    }

    public static class MergeExtensions
    {
        private static readonly ILogger l = Log.Get();

        public static bool IsMergeable(this Type type)
        {
            return typeof(IMergeable).IsAssignableFrom(type);
        }

        public static object GetDefaultValue(PropertyInfo mi)
        {
            object defaultValue;
            try
            {
                var defaultAttr = mi.GetCustomAttribute<DefaultValueAttribute>();
                if (defaultAttr != null)
                {
                    defaultValue = defaultAttr.Value;
                }
                else
                {
                    if (!mi.PropertyType.IsValueType)
                    {
                        defaultValue = null;
                    }
                    else
                    {
                        defaultValue = Activator.CreateInstance(mi.PropertyType);
                    }
                }
            }
            catch (Exception)
            {
#if true
                throw;
#else
                //var msg = "UNEXPECTED: Exception when trying to determine default value for value type '" + mi.PropertyType.FullName + "': " + ex;
                //l.Error(msg);
                //defaultValue = null;
#endif
            }
            return defaultValue;
        }

        /// <summary>
        /// Merge field and property values from source into target.
        /// NOTE: Only Properties are deeply merged.  Public fields are always copied.
        /// </summary>
        /// <remarks>
        /// Not supported: index parameters
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="source"></param>
        public static void MergeFrom<T>(this T target, T source, MergeOptions mergeOptions = MergeOptions.DeepForIMergeableOnly)
        {
            if (source == null) return;

            foreach (PropertyInfo mi in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!mi.CanRead || !mi.CanWrite) continue;
                if (mi.GetIndexParameters().Length > 0)
                {
                    //System.Diagnostics.Trace.WriteLine("");
                    continue;
                }

                {
                    var attr = mi.GetCustomAttribute<OverlayIgnoreAttribute>();
                    if (attr != null) continue;
                }

                Type propertyTypeGeneric = mi.PropertyType.IsGenericType ? mi.PropertyType.GetGenericTypeDefinition() : null;

                object sourceValue = mi.GetValue(source, null);
                if (sourceValue == null && propertyTypeGeneric == typeof(Nullable<>))
                {
                    continue;
                }

                object defaultValue = GetDefaultValue(mi);
                if (sourceValue == null)
                {
                    if (defaultValue == null) continue;
                }
                else if (sourceValue.Equals(defaultValue)) continue;

                if (mergeOptions != MergeOptions.Shallow)
                {
                    if (typeof(IMergeable).IsAssignableFrom(mi.PropertyType))
                    {
                        IMergeable mergeable = (IMergeable)mi.GetValue(target, null);
                        mergeable.MergeFrom(mi.GetValue(source, null));
                        continue;
                    }
                    else if (propertyTypeGeneric == typeof(Nullable<>) && typeof(IMergeable).IsAssignableFrom(mi.PropertyType.GetGenericArguments()[0]))
                    {
                        IMergeable mergeable = (IMergeable)mi.GetValue(target, null);
                        if (mergeable == null)
                        {
                            try
                            {
                                mergeable = (IMergeable)Activator.CreateInstance(mi.PropertyType);
                            }
                            catch { }
                        }
                        if (mergeable != null)
                        {
                            mergeable.MergeFrom(mi.GetValue(source, null));
                        }
                        continue;
                    }
                }


                if (mi.PropertyType.IsValueType || mergeOptions != MergeOptions.Deep)
                {
                    // DeepForIMergeableOnly gets handled here as shallow when it's not IMergeable
                    
                    l.Trace("UNTESTED - shallow/value merge with options - " + mergeOptions + " - " + target + " " + Environment.StackTrace);
                    // Shallow:
                    mi.SetValue(target, sourceValue, null);
                    continue;
                }
                
#if SanityChecks
                if (typeof(IMergeable).IsAssignableFrom(mi.PropertyType)) throw new UnreachableCodeException();
#endif

                // If the option is Deep, always try to merge deep for non-value types and non-IMergeable types:
                l.Trace("UNTESTED - recursive merge");
                MergeFrom(mi.GetValue(target, null), mi.GetValue(source, null), mergeOptions);

            }

            foreach (FieldInfo mi in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (mi.IsInitOnly) continue;

                {
                    var attr = mi.GetCustomAttribute<OverlayIgnoreAttribute>();
                    if (attr != null) continue;
                }

                {
                    var attr = typeof(T).GetCustomAttribute<OverlayIgnoreAttribute>();
                    if (attr != null) continue;
                }

                // TODO FIXME REVIEW - use same merge logic as properties??

                Type propertyTypeGeneric = mi.FieldType.IsGenericType ? mi.FieldType.GetGenericTypeDefinition() : null;

                object sourceValue = mi.GetValue(source);
                if (sourceValue == null && propertyTypeGeneric == typeof(Nullable<>))
                {
                    continue;
                }
                mi.SetValue(target, sourceValue);
            }
        }

    }
}
