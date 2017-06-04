#if OX
// TODO REFACTOR REVIEW - Don't use ICloneable, and review overlap with AssignFrom and CopyFrom  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using OX.Copyable;

namespace LionFire.Reflection.Assignment
{
    public static class CopyExtensions
    {
        public static object Copy(this object me, CopyFlags copyFlags = CopyFlagsSettings.Default)
        {
            ICloneable c = me as ICloneable;
            if (c != null) return c.Clone();

            return c.DeepCopy(copyFlags);
        }

        public static T Copy<T>(this T me, CopyFlags copyFlags = CopyFlagsSettings.Default)
            where T : class
        {
            ICloneable c = me as ICloneable;
            if (c != null) return (T)c.Clone();

            return me.DeepCopy<T>(copyFlags);
        }
    }
}
#endif