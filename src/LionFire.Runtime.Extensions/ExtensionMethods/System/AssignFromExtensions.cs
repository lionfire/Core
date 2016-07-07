//#define LFU // Imported from LionFire.Utility
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods
{
    public static class AssignFromExtensions
    {
        public static object AssignPropertiesFrom(this object me, object other)
        {
            Type T = me.GetType();
            foreach (PropertyInfo mi in T.GetProperties())
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

                object val = mi.GetValue(other, null);

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
                mi.SetValue(me, val, null);
#endif
            }
            return me;
        }

        public static object AssignPropertiesTo(this object me, object other)
        {
            Type T = me.GetType();
            foreach (PropertyInfo mi in T.GetProperties())
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

                object val = mi.GetValue(me, null);

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
