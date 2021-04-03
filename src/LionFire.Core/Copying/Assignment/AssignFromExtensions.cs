//#define LFU // Imported from LionFire.Utility
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Copying;

namespace LionFire.ExtensionMethods.Copying
{
    public static class AssignFromExtensions
    {
        #region AssignFrom

        public static void AssignFrom(this object me, object other, AssignmentMode assignmentMode)
        {
            if (other == null)
            {
                return;
            }

            if (me == null)
            {
                throw new ArgumentNullException($"{nameof(me)}");
            }

            AssignFrom_(me, other, assignmentMode);
        }
        public static void AssignFrom<T>(this T me, T other, AssignmentMode assignmentMode)
        {
            if (other == null)
            {
                return;
            }

            if (me == null)
            {
                throw new ArgumentNullException($"{nameof(me)}");
            }

            AssignFrom_(me, other, assignmentMode);
        }

        #endregion

        #region Properties / Fields

        public static object AssignPropertiesAndFieldsFromDictionary(this object me, Dictionary<string, object> dict, bool includeNonPublic = false)
        {
            Type myType = me.GetType();

            foreach (var kvp in dict)
            {
                {
                    var mi = myType.GetProperty(kvp.Key, BindingFlags.Public | (includeNonPublic ? BindingFlags.NonPublic : BindingFlags.Default));
                    if (mi != null)
                    {
                        mi.SetValue(me, kvp.Value);
                        continue;
                    }
                }
                {
                    var mi = myType.GetField(kvp.Key, BindingFlags.Public | (includeNonPublic ? BindingFlags.NonPublic : BindingFlags.Default));
                    if (mi != null)
                    {
                        mi.SetValue(me, kvp.Value);
                        continue;
                    }
                }
            }
            return me;
        }

        public static object AssignPropertiesFrom(this object me, Dictionary<string, object> dict)
        {
            Type myType = me.GetType();

            foreach (var kvp in dict)
            {
                var pi = myType.GetProperty(kvp.Key);
                if (pi == null)
                {
                    Debug.WriteLine($"WARN - AssignPropertiesFrom: property {kvp.Key} not found on object {me.GetType().FullName}");
                    continue;
                }
                var val = kvp.Value;
                if (pi.PropertyType.GetTypeInfo().IsEnum)
                {
                    if (val is string)
                    {
                        val = Enum.Parse(pi.PropertyType, (string)val);
                    }
                    else
                    {
                        val = Enum.ToObject(pi.PropertyType, val);
                    }
                }
                pi.SetValue(me, val);
            }
            return me;
        }

        public static object AssignNonDefaultPropertiesFrom(this object me, object other) => AssignPropertiesFrom(me, other, (m, o, pi) => pi.GetValue(o) != Activator.CreateInstance(pi.PropertyType));
        public static object AssignNonNullPropertiesFrom(this object me, object other) => AssignPropertiesFrom(me, other, (m, o, pi) => pi.GetValue(o) != null);

        public static object AssignPropertiesFrom(this object me, object other, Func<object,object, PropertyInfo, bool> filter = null)
        {
            Type myType = me.GetType();
            Type otherType = other.GetType();
            var sameType = myType == otherType;

            foreach (PropertyInfo otherMethodInfo in otherType.GetProperties())
            {
                if (!filter(me, other, otherMethodInfo)) continue;
                me.AssignPropertyFrom(other, otherMethodInfo);
            }
            return me;
        }
        public static object AssignPropertiesFrom(this object me, object other)
        {
            Type myType = me.GetType();
            Type otherType = other.GetType();
            bool sameType = myType == otherType;

            foreach (PropertyInfo otherMethodInfo in otherType.GetProperties())
            {
                me.AssignPropertyFrom(other, otherMethodInfo);
            }
            return me;
        }
        public static object AssignFieldsFrom(this object me, object other)
        {
            Type myType = me.GetType();
            Type otherType = other.GetType();
            bool sameType = myType == otherType;

            // TOOPTIMIZE - iterate through othertype.
            foreach (FieldInfo mi in myType.GetFields())
            {
                me.AssignFieldFrom(other, mi);
            }
            return me;
        }

        #region Individual Property/Field

        public static void AssignPropertyFrom(this object me, object other, PropertyInfo otherPropertyInfo)
        {
            Type myType = me.GetType();
            Type otherType = other.GetType();
            bool sameType = myType == otherType;

            if (!otherPropertyInfo.CanRead)
            {
                return;
            }

            if (otherPropertyInfo.GetIndexParameters().Length > 0)
            {
                return;
            }
#if NET35
                if (mi.GetGetMethod().IsStatic) return;
#else
            if (otherPropertyInfo.GetMethod.IsStatic)
            {
                return;
            }
#endif

#if LFU
                AssignmentMode miMode = GetAssignmentMode(mi, assignmentMode);
                if (miMode == AssignmentMode.Ignore) continue;
#endif

            // TODO: Copy this same logic to AssignProperties to, and field methods
            var myPi = sameType ? otherPropertyInfo : myType.GetProperty(otherPropertyInfo.Name);

            if (myPi == null || myPi.PropertyType != otherPropertyInfo.PropertyType)
            {
                return;
            }
            if (!myPi.CanWrite)
            {
                return;
            }

            object val = otherPropertyInfo.GetValue(other, null);

#if LFU
                //if (GetAssignmentValue(mi, other, useICloneableIfAvailable, ref val))
                if (GetAssignmentValue(mi, other, miMode, ref val))
                {
#if TRACE_ASSIGNFROM
                    l.Debug("[ASSIGNFROM] " + T.Name + "." + mi.Name + " = " + val);
#endif
                    mi.SetValue(me, val, null);
                }
#if TRACE_ASSIGNFROM
                else
                {
                    l.Trace("[ASSIGNFROM] Skipping " + T.Name + "." + mi.Name);
                }
#endif
#else
            //Trace.WriteLine(me.GetType().Name + "." + myPi.Name + " = " + other.GetType() + "." + otherPropertyInfo.Name + " (" + val + ")");
            myPi.SetValue(me, val, null);
#endif
        }
        public static void AssignFieldFrom(this object me, object other, FieldInfo mi)
        {
            Type myType = me.GetType();
            Type otherType = other.GetType();
            bool sameType = myType == otherType;

            if (mi.IsInitOnly)
            {
                return;
            }

#if NET35
                if (mi.IsStatic.IsStatic) return;
#else
            if (mi.IsStatic)
            {
                return;
            }
#endif

#if LFU
                AssignmentMode miMode = GetAssignmentMode(mi, assignmentMode);
                if (miMode == AssignmentMode.Ignore) continue;
#endif

            // TODO: Copy this same logic to AssignProperties to, and field methods
            var otherMi = sameType ? mi : otherType.GetField(mi.Name);

            if (otherMi == null || otherMi.FieldType != mi.FieldType)
            {
                return;
            }

            object val = otherMi.GetValue(other);

#if LFU
                //if (GetAssignmentValue(mi, other, useICloneableIfAvailable, ref val))
                if (GetAssignmentValue(mi, other, miMode, ref val))
                {
#if TRACE_ASSIGNFROM
                    l.Debug("[ASSIGNFROM] " + T.Name + "." + mi.Name + " = " + val);
#endif
                    mi.SetValue(me, val, null);
                }
#if TRACE_ASSIGNFROM
                else
                {
                    l.Trace("[ASSIGNFROM] Skipping " + T.Name + "." + mi.Name);
                }
#endif
#else
            mi.SetValue(me, val);
#endif
        }
        
        #endregion
        
        #endregion
        
        public static object AssignPropertiesTo(this object me, object other, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            Type T = other.GetType();
            Type myType = me.GetType();
            foreach (PropertyInfo mi in T.GetProperties(bindingFlags))
            {
                if (!mi.CanRead || !mi.CanWrite)
                {
                    continue;
                }

                if (mi.GetIndexParameters().Length > 0)
                {
                    continue;
                }

#if NET35
                if (mi.GetGetMethod().IsStatic) continue;
#else
                if (mi.GetMethod.IsStatic)
                {
                    continue;
                }
#endif

#if LFU
                AssignmentMode miMode = GetAssignmentMode(mi, assignmentMode);
                if (miMode == AssignmentMode.Ignore) continue;
#endif

                var myMethodInfo = myType.GetProperty(mi.Name, bindingFlags);
                if (myMethodInfo == null)
                {
                    continue;
                }

                object val = myMethodInfo.GetValue(me);

#if LFU
#error Switch me/other
                //if (GetAssignmentValue(mi, other, useICloneableIfAvailable, ref val))
                if (GetAssignmentValue(mi, other, miMode, ref val))
                {
#if TRACE_ASSIGNFROM
                    l.Debug("[ASSIGNFROM] " + T.Name + "." + mi.Name + " = " + val);
#endif
                    mi.SetValue(me, val, null);
                }
#if TRACE_ASSIGNFROM
                else
                {
                    l.Trace("[ASSIGNFROM] Skipping " + T.Name + "." + mi.Name);
                }
#endif
#else
                mi.SetValue(other, val, null);
#endif
            }
            return me;
        }
        
        public static void AssignArrayFrom(this Array me, Array other, AssignmentMode assignmentMode = AssignmentMode.Assign)
        {
            if (me.GetType().IsArray)
            {
                Array myArray = me;
                Array copy = other;
                switch (assignmentMode)
                {
                    //case AssignmentMode.Unspecified:
                    //break;
                    case AssignmentMode.Ignore:
                        break;
                    case AssignmentMode.Assign:
                        Array.Copy(me, other, myArray.Length);
                        break;
                    case AssignmentMode.CloneIfCloneable:
                        // REVIEW - CloneIfCloneable otherwise DeepCopy?  What if user wants CloneOrShallowCopy?
                        for (int i = 0; i < myArray.Length; i++)
                        {
                            copy.SetValue(myArray.GetValue(i).Copy(), i);
                        }
                        break;
                    case AssignmentMode.DeepCopy:
                        for (int i = 0; i < myArray.Length; i++)
                        {
                            copy.SetValue(myArray.GetValue(i).DeepCopy(), i);
                        }
                        break;
                    default:
                        throw new ArgumentException("mode");
                }

                return;
            }
        }

        #region Convenience

        public static void ShallowAssignFrom(this object me, object other) => AssignFrom(me, other, AssignmentMode.Assign);
        
        #endregion

        #region Imported from LFU
        
        // Move to generic extension method?
        //private static void AssignFrom_(Type T, object me, object other, bool useICloneableIfAvailable = false) // Move to generic extension method?
        private static void AssignFrom_(
            //Type T, 
            object me, object other, AssignmentMode assignmentMode = AssignmentMode.DeepCopy)
        {
            if (me.GetType().IsArray)
            {
                AssignArrayFrom((Array)me, (Array)other, assignmentMode);
                return;
            }

            #region TODO: Use/merge with AssignPropertiesFrom?

            //me.AssignPropertiesFrom(other);

            Type T = me.GetType();
            foreach (PropertyInfo mi in T.GetProperties())
            {
                if (!mi.CanRead || !mi.CanWrite)
                {
                    continue;
                }

                if (mi.GetIndexParameters().Length > 0)
                {
                    continue;
                }
#if NET35
                if (mi.GetGetMethod().IsStatic) continue;
#else
                if (mi.GetMethod.IsStatic)
                {
                    continue;
                }
#endif

                AssignmentMode miMode = CopyFromExtensions.GetAssignmentMode(mi, assignmentMode);
                if (miMode == AssignmentMode.Ignore)
                {
                    continue;
                }

                object val = mi.GetValue(other, null);

                //if (GetAssignmentValue(mi, other, useICloneableIfAvailable, ref val))
                if (CopyFromExtensions.GetAssignmentValue(mi, other, miMode, ref val))
                {
#if TRACE_ASSIGNFROM
                    l.Debug("[ASSIGNFROM] " + T.Name + "." + mi.Name + " = " + val);
#endif
                    mi.SetValue(me, val, null);
                }
#if TRACE_ASSIGNFROM
                else
                {
                    l.Trace("[ASSIGNFROM] Skipping " + T.Name + "." + mi.Name);
                }
#endif
            }

            #endregion

            #region TODO: Use/merge with AssignFieldsFrom?

            foreach (FieldInfo mi in T.GetFields())
            {
                //if (mi.IsInitOnly) continue;
                if (mi.IsLiteral)
                {
                    continue;
                }

                if (mi.IsStatic)
                {
                    continue;
                }
                //if (mi.GetCustomAttribute<AssignIgnoreAttribute>() != null) continue;

                AssignmentMode miMode = CopyFromExtensions.GetAssignmentMode(mi, assignmentMode);
                if (miMode == AssignmentMode.Ignore)
                {
                    continue;
                }

                //var attr = mi.GetCustomAttribute<AssignmentAttribute>();
                //if (attr != null && attr.AssignmentMode == AssignmentMode.Ignore) continue;

                //object val = mi.GetValue(other);
                //if (useICloneableIfAvailable)
                //{
                //    ICloneable cloneable = val as ICloneable;
                //    if (cloneable != null)
                //    {
                //        val = cloneable.Clone();
                //        continue;
                //    }
                //}

                object val = mi.GetValue(other);

                //if (GetAssignmentValue(mi, other, useICloneableIfAvailable, ref val))
                if (CopyFromExtensions.GetAssignmentValue(mi, other, miMode, ref val))
                {
#if TRACE_ASSIGNFROM
                    l.Debug("[ASSIGNFROM] " + T.Name + "." + mi.Name + " = " + val);
#endif
                    mi.SetValue(me, val);
                }
#if TRACE_ASSIGNFROM
                else
                {
                    l.Trace("[ASSIGNFROM] Skipping " + T.Name + "." + mi.Name);
                }
#endif
            }

            #endregion

        }

        #endregion
    }
}