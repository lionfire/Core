using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// REVIEW - Look at OX.Copyable;

namespace LionFire.Copying
{
    public static class CopyExtensions
    {
        /// <summary>
        /// Clone if possible, otherwise DeepCopy.  
        /// REVIEW - change method name to CloneOrDeepCopy?  Or does copyFlags allow for magic?
        /// </summary>
        /// <param name="me"></param>
        /// <param name="copyFlags"></param>
        /// <returns></returns>
        public static object Copy(this object me, CopyFlags copyFlags = CopyFlagsSettings.Default)
        {
            ICloneable c = me as ICloneable;
            if (c != null) return c.Clone();

            return c.DeepCopy(copyFlags);
        }

        /// <summary>
        /// Clone if possible, otherwise DeepCopy.  
        /// REVIEW - change method name to CloneOrDeepCopy?  Or does copyFlags allow for magic?
        /// </summary>
        /// <param name="me"></param>
        /// <param name="copyFlags"></param>
        /// <returns></returns>
        public static T Copy<T>(this T me, CopyFlags copyFlags = CopyFlagsSettings.Default)
            where T : class
        {
            ICloneable c = me as ICloneable;
            if (c != null) return (T)c.Clone();

            return me.DeepCopy<T>(copyFlags);
        }
    }
}
