using System;
using System.Collections.Generic;
using LionFire.Copying;
using LionFire.ExtensionMethods;
using LionFire.ExtensionMethods.Copying;


namespace LionFire.ExtensionMethods.Cloning
{
    public static class CloningExtensions
    {
#if !AOT
        // FUTURE?
        //public static T Clone<T>(T me)
        //    where T : class, new()
        //{
        //    T result = new T();
        //    result.AssignFrom(me, true, AssignmentMode.DeepCopy);

        //    return result;
        //}
#endif
        //public static object Clone(this object me, bool cloneCloneableMembers = true)
        //{
        //    if (me == null) throw new ArgumentNullException("me");

        //    object result = Activator.CreateInstance(me.GetType());
        //    result.AssignFrom(me, cloneCloneableMembers);

        //    return result;
        //}

#if DEEPCOPY // Use my AssignFrom
        public static object Clone(this object me, AssignmentMode assignmentMode = AssignmentMode.DeepCopy)
        {
            if (me == null) throw new ArgumentNullException("me");

            var result = me.DeepCopy();

            return result;
        }
        public static T Clone<T>(this T me, AssignmentMode assignmentMode = AssignmentMode.DeepCopy)
            where T : class //, new()
        {
            if (me == null) throw new ArgumentNullException("me");

            var result = me.DeepCopy();
            
            return result;
        }
#else
        public static object Clone(this object me, AssignmentMode copyMode = AssignmentMode.DeepCopy)
        {
            if (me == null) throw new ArgumentNullException("me");

            object result = Activator.CreateInstance(me.GetType());
            result.CopyFrom(me, copyMode);

            return result;
        }
        public static T Clone<T>(this T me, AssignmentMode copyMode = AssignmentMode.DeepCopy)
            where T : class
        {
            if (me == null) throw new ArgumentNullException("me");

            T result = (T)Activator.CreateInstance(me.GetType());
            result.CopyFrom(me, copyMode);

            return result;
        }
#endif
    }
}