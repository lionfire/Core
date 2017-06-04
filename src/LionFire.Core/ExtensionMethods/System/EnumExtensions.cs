using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.ExtensionMethods
{
    public static class EnumExtensions
    {
        public static bool HasAnyFlag(this Enum enum1, Enum enum2)
        {
            var obj1 = Convert.ToInt64(Convert.ChangeType(enum1, Enum.GetUnderlyingType(enum1.GetType())));
            var obj2 = Convert.ToInt64(Convert.ChangeType(enum2, Enum.GetUnderlyingType(enum1.GetType())));

            return (obj1 & obj2) != 0;
        }
    }
}
