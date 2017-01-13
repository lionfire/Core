using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LionFire
{
    // Move to .NET Core LF.Utility
    public static class XamlUtilities
    {
        public static string ToXamlAttribute<T>(this T obj, params string[] propertyNames)
        {
            if (obj == null)
            {
                return $"{{null ({obj.GetType().Name})}}";
            }

            var sb = new StringBuilder();
            sb.Append($"{{{obj.GetType().Name }");

            HashSet<string> whitelist = propertyNames.Length > 0 ? new HashSet<string>(propertyNames) : null;

            bool firstProperty = true;
            foreach (var pi in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (whitelist != null && !whitelist.Contains(pi.Name)) continue;

                if (firstProperty)
                {
                    firstProperty = false;
                }
                else
                {
                    sb.Append(",");
                }

                sb.Append(" ");
                sb.Append(pi.Name);
                sb.Append("=");
                sb.Append(pi.GetValue(obj).ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

}
