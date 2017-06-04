using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods
{
    public static class BooleanUtils
    {
        public static bool? ToTernary(this IEnumerable<bool> list)
        {
            if (list == null || !list.Any()) return null;

            bool? result = list.First();
            foreach (var item in list.Skip(1))
            {
                if (item != result.Value)
                {
                    result = null;
                    break;
                }
            }
            return result;
        }
    }
}
