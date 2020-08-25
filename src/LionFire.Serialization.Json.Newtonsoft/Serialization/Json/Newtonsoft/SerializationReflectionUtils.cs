using System;
using System.Reflection;

namespace LionFire.Serialization
{

    public static class SerializationReflectionUtils
    {
        public static bool ShouldSerialize(this PropertyInfo propertyInfo, LionSerializeContext context)
        {
            if (!propertyInfo.CanWrite)
            {
                if (context.HasFlag(LionSerializeContext.External)) return true;
                else return false;
            }
            foreach (var attr in propertyInfo.GetCustomAttributes<IgnoreAttribute>())
            {
                if ((attr.Ignore | context) != LionSerializeContext.None) return false;
            }


            return true;
        }
    }
}
