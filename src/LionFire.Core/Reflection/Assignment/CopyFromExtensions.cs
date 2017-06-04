//#define DEEPCOPY
//#define COPYABLE
//#define TRACE_ASSIGNFROM
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
#if COPYABLE
using OX.Copyable;
#endif

namespace LionFire.ExtensionMethods.CopyFrom
{
    public static class CopyFromExtensions
    {
#if !AOT
        /// <summary>
        /// FUTURE: More options about private/InitOnly members
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="me"></param>
        /// <param name="other"></param>
        /// <param name="useICloneableIfAvailable"></param>
        public static void CopyFrom<T>(this T me, T other, AssignmentMode assignmentMode)
            where T : class
        {
            if (other == null) return;
            if (me == null) throw new ArgumentNullException("me == null");

            AssignFrom_(me, other, assignmentMode);

            //foreach (PropertyInfo mi in typeof(T).GetProperties())
            //{
            //    if (!mi.CanRead || !mi.CanWrite) continue;
            //    if (mi.GetIndexParameters().Length > 0) continue;
            //    if (mi.GetCustomAttribute<AssignIgnoreAttribute>() != null) continue;

            //    var attr = mi.GetCustomAttribute<AssignmentAttribute>();
            //    if (attr != null && attr.CloneSettings == AssignmentMode.Ignore) continue;

            //    object val = mi.GetValue(other, null);

            //    if (useICloneableIfAvailable)
            //    {
            //        ICloneable cloneable = val as ICloneable;
            //        if (cloneable != null)
            //        {
            //            val = cloneable.Clone();
            //            continue;
            //        }
            //    }

            //    mi.SetValue(me, val, null);
            //}

            //foreach (FieldInfo mi in typeof(T).GetFields())
            //{
            //    //if (mi.IsInitOnly) continue;
            //    if (mi.GetCustomAttribute<AssignIgnoreAttribute>() != null) continue;

            //    var attr = mi.GetCustomAttribute<AssignmentAttribute>();
            //    if (attr != null && attr.CloneSettings == AssignmentMode.Ignore) continue;

            //    object val = mi.GetValue(other);

            //    if (useICloneableIfAvailable)
            //    {
            //        ICloneable cloneable = val as ICloneable;
            //        if (cloneable != null)
            //        {
            //            val = cloneable.Clone();
            //            continue;
            //        }
            //    }

            //    mi.SetValue(me, val);
            //}
        }
#endif

        #region Private

        private static bool GetAssignmentValue(MemberInfo mi, object other,
            AssignmentMode assignmentMode,
            //bool useICloneableIfAvailable, 
            ref object val)
        {
            var attr2 = mi.GetCustomAttribute<IgnoreAttribute>();
            if (attr2 != null && (attr2.Ignore & LionSerializeContext.Copy) != LionSerializeContext.None) return false;

            var attr = mi.GetCustomAttribute<AssignmentAttribute>();

            //AssignmentMode mode = attr != null ? attr.AssignmentMode : (useICloneableIfAvailable ? AssignmentMode.CloneIfCloneable : AssignmentMode.Assign);
            AssignmentMode mode = attr != null ? attr.AssignmentMode : assignmentMode;

            if (attr != null && attr.AssignmentMode == AssignmentMode.Ignore) return false;

            switch (mode)
            {
                case AssignmentMode.Unspecified:
                default:
                    throw new ArgumentException("Invalid AssignmentMode on member " + mi.Name);
                case AssignmentMode.Ignore:
                    return false;
                case AssignmentMode.Assign:
                    break;
                case AssignmentMode.CloneIfCloneable:
#if ICloneable
                    {
                        ICloneable cloneable = val as ICloneable;
                        if (cloneable == null)
                        {
                            // Leave val as the original value, and use it as it is used in the assign mode
                        }
                        else
                        {
                            val = cloneable.Clone();
                        }
                        break;
                    }
#else
                    // https://blogs.msdn.microsoft.com/brada/2004/05/03/should-we-obsolete-icloneable-the-slar-on-system-icloneable/
                    throw new NotImplementedException("ICloneable no longer supported");
#endif         
                case AssignmentMode.DeepCopy:
                    {
                        if (val == null || val.GetType().GetTypeInfo().IsValueType)
                        {
                            goto case AssignmentMode.Assign;
                        }
#if ICloneable
                        ICloneable cloneable = val as ICloneable;
                        if (cloneable == null)
                        {

                            val = LionFire.Extensions.Cloning.CloningExtensions.Clone(val);
                            //throw new ArgumentException("AssignmentMode is DeepCopy but value is not ICloneable: " + val.GetType().FullName);
                        }
                        else
                        {
                            val = cloneable.Clone();
                        }
#else
                        val = LionFire.ExtensionMethods.Cloning.CloningExtensions.Clone(val);
#endif
                        break;
                    }
            }
            return true;
        }

        // TODO - streamline down to one attribute?
        private static AssignmentMode GetAssignmentMode(MemberInfo mi, AssignmentMode def)
        {
            AssignmentMode miMode = def;

            if (mi.GetCustomAttribute<AssignIgnoreAttribute>() != null) return AssignmentMode.Ignore;

            var attr = mi.GetCustomAttribute<IgnoreAttribute>();
            if (attr != null && attr.Ignore.HasFlag(LionSerializeContext.Copy)) return AssignmentMode.Ignore;

            var attr2 = mi.GetCustomAttribute<AssignmentAttribute>();

            if (attr2 != null)
            {
                miMode = attr2.AssignmentMode;
            }

            return miMode;
        }

        public static void CopyArrayFrom(Array me, Array other, AssignmentMode assignmentMode = AssignmentMode.DeepCopy)
        {
            Array myArray = (Array)me;
            Array copy = (Array)other;
            switch (assignmentMode)
            {
                case AssignmentMode.Ignore:
                    break;
                case AssignmentMode.Assign:
                    Array.Copy((Array)me, (Array)other, myArray.Length);
                    break;
                case AssignmentMode.CloneIfCloneable:
#if OX
                    for (int i = 0; i < myArray.Length; i++)
                    {
                        copy.SetValue(myArray.GetValue(i).Copy(), i);
                    }
                    break;
#else
                    throw new NotImplementedException("TODO");
#endif
                case AssignmentMode.DeepCopy:
#if OX
                    for (int i = 0; i < myArray.Length; i++)
                    {
                        copy.SetValue(myArray.GetValue(i).DeepCopy(), i);
                    }
                    break;
#else
                    throw new NotImplementedException("TODO");
#endif
                //case AssignmentMode.Unspecified:
                default:
                    throw new ArgumentException("mode");
            }
        }

        // Move to generic extension method?
        private static void AssignFrom_(object me, object other, AssignmentMode assignmentMode = AssignmentMode.DeepCopy)
        {
            if (me.GetType().IsArray)
            {
                if (!(other is Array)) { throw new ArgumentException("me is Array but other is not"); }
                CopyArrayFrom((Array)me, (Array)other, assignmentMode);
                return;
            }

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

                AssignmentMode miMode = GetAssignmentMode(mi, assignmentMode);
                if (miMode == AssignmentMode.Ignore) continue;

                object val = mi.GetValue(other, null);

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
            }

            foreach (FieldInfo mi in T.GetFields())
            {
                //if (mi.IsInitOnly) continue;
                if (mi.IsLiteral) continue;
                if (mi.IsStatic) continue;
                //if (mi.GetCustomAttribute<AssignIgnoreAttribute>() != null) continue;

                AssignmentMode miMode = GetAssignmentMode(mi, assignmentMode);
                if (miMode == AssignmentMode.Ignore) continue;

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
                if (GetAssignmentValue(mi, other, miMode, ref val))
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
        }

        //#if !AOT
        //public static void AssignFrom(this object me, object other, bool useICloneableIfAvailable = false) // Move to generic extension method?
        //{
        //    if (other == null) return;
        //    if (me == null) throw new ArgumentNullException("me == null");
        //    Type T = me.GetType();
        //    AssignFrom_(T, me, other, useICloneableIfAvailable);
        //}

#endregion

#region Convenience

        public static void ShallowCopyFrom(this object me, object other)
        {
            CopyFrom(me, other, AssignmentMode.Assign);
        }

#endregion

        public static void CopyFrom(this object me, object other, AssignmentMode assignmentMode) // Move to generic extension method?
        {

            if (other == null) return;
            if (me == null) throw new ArgumentNullException("me == null");

            AssignFrom_(me, other, assignmentMode);
        }

#region Misc

        //private static ILogger l = Log.Get();

#endregion
    }
}
