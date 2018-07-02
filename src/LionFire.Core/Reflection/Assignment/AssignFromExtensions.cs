//#define LFU // Imported from LionFire.Utility
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods
{
    public static class AssignFromExtensions
    {
        public static object AssignFrom(this object me, Dictionary<string, object> dict)
        {
            Type myType = me.GetType();

            foreach (var kvp in dict)
            {
                {
                    var mi = myType.GetProperty(kvp.Key);
                    if (mi != null)
                    {
                        mi.SetValue(me, kvp.Value);
                        continue;
                    }
                }
                {
                    var mi = myType.GetField(kvp.Key);
                    if (mi != null)
                    {
                        mi.SetValue(me, kvp.Value);
                        continue;
                    }
                }
            }
            return me;
        }

        public static object AssignPropertiesFrom(this object me, Dictionary<string,object> dict)
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


        public static void AssignPropertyFrom(this object me, object other, PropertyInfo otherPropertyInfo)
        {
            Type myType = me.GetType();
            Type otherType = other.GetType();
            bool sameType = myType == otherType;

            if (!otherPropertyInfo.CanRead || !otherPropertyInfo.CanWrite) return;
            if (otherPropertyInfo.GetIndexParameters().Length > 0) return;
#if NET35
                if (mi.GetGetMethod().IsStatic) return;
#else
            if (otherPropertyInfo.GetMethod.IsStatic) return;
#endif

#if LFU
                AssignmentMode miMode = GetAssignmentMode(mi, assignmentMode);
                if (miMode == AssignmentMode.Ignore) continue;
#endif

            // TODO: Copy this same logic to AssignProperties to, and field methods
            var myPi = sameType ? otherPropertyInfo : myType.GetProperty(otherPropertyInfo.Name);

            if (myPi == null || myPi.PropertyType != otherPropertyInfo.PropertyType) return;

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

            if (mi.IsInitOnly) return; 

#if NET35
                if (mi.IsStatic.IsStatic) return;
#else
            if (mi.IsStatic) return;
#endif

#if LFU
                AssignmentMode miMode = GetAssignmentMode(mi, assignmentMode);
                if (miMode == AssignmentMode.Ignore) continue;
#endif

            // TODO: Copy this same logic to AssignProperties to, and field methods
            var otherMi = sameType ? mi : otherType.GetField(mi.Name);

            if (otherMi == null || otherMi.FieldType != mi.FieldType) return;

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

        public static object AssignPropertiesTo(this object me, object other, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            Type T = other.GetType();
            Type myType = me.GetType();
            foreach (PropertyInfo mi in T.GetProperties(bindingFlags))
            {
                if (!mi.CanRead || !mi.CanWrite) continue;
                if (mi.GetIndexParameters().Length > 0) continue;

#if NET35
                if (mi.GetGetMethod().IsStatic) continue;
#else
                if (mi.GetMethod.IsStatic) continue;
#endif

#if LFU
                AssignmentMode miMode = GetAssignmentMode(mi, assignmentMode);
                if (miMode == AssignmentMode.Ignore) continue;
#endif

                var myMethodInfo = myType.GetProperty(mi.Name, bindingFlags);
                if (myMethodInfo == null) continue;


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
    }
}
