using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LionFire.Reflection
{
    public static class ReflectionUtils
    {
        public static bool IsPropertyMethodName(string methodName)
        {
            return methodName.StartsWith("get_") || methodName.StartsWith("set_");
        }
        public static bool IsPropertyMethod(this MethodInfo methodInfo)
        {
            return IsPropertyMethodName(methodInfo.Name);
        }

        public static PropertyInfo GetProperty(this MethodInfo methodInfo)
        {
            if (!methodInfo.IsPropertyMethod())
            {
                return null;
            }

            PropertyInfo propertyInfo = methodInfo.DeclaringType.GetProperty(methodInfo.Name.Substring("get_".Length));
            return propertyInfo;
        }
    }
}
