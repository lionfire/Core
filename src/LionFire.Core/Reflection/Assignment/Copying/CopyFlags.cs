#if OX
// TODO REFACTOR REVIEW - Don't use ICloneable, and review overlap with AssignFrom and CopyFrom  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire
{
    [Flags]
    public enum CopyFlags
    {
        None = 0,

        PublicProperties = 1 << 0,
        NonPublicProperties = 1 << 1,

        PublicFields = 1 << 2,
        NonPublicFields = 1 << 3,

        ICloneable = 1 << 4,

        ICloneProvider = 1 << 5,

        /// <summary>
        /// Don't do a deep copy
        /// </summary>
        Shallow = 1 << 8,

        AllProperties = PublicFields | PublicProperties,
        AllFields = PublicFields | NonPublicFields,

        All = AllProperties | AllFields,
        AllWithICloneable = All | ICloneable,
    }

    public static class CopyFlagsSettings
    {
        //public const CopyFlags Default = CopyFlags.ICloneable | CopyFlags.PublicProperties;

        public const CopyFlags Default =
            CopyFlags.ICloneable
            | CopyFlags.ICloneProvider
            | CopyFlags.PublicProperties | CopyFlags.PublicFields
            //| CopyFlags.NonPublicProperties | CopyFlags.NonPublicFields  // RECENTCHANGE
            ;
    }
}
#endif