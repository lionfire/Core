using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LionFire
{
    public static class XamlUtilities
    {
        public static string ToXamlProperties<T>(this T obj, params string[] propertyNames)
        {
            var sb = new StringBuilder();
            ToXamlProperties(obj, sb, propertyNames);
            return sb.ToString();
        }

        public static void ToXamlProperties<T>(this T obj, StringBuilder stringBuilder, params string[] propertyNames)
        {
            if (obj is null) { return; }


            HashSet<string> whitelist = propertyNames.Length > 0 ? new HashSet<string>(propertyNames) : null;

            bool firstProperty = true;
            foreach (var pi in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (whitelist != null && !whitelist.Contains(pi.Name)) continue;

                var val = pi.GetValue(obj);
                if (object.ReferenceEquals(obj, val)) continue;
                if (val == null) continue;

                if (firstProperty)
                {
                    firstProperty = false;
                }
                else
                {
                    stringBuilder.Append(",");
                }

                stringBuilder.Append(" ");
                stringBuilder.Append(pi.Name);
                stringBuilder.Append("=");
                stringBuilder.Append(pi.GetValue(obj)?.ToString());
            }
        }

        public static string ToXamlAttribute<T>(this T obj, params string[] propertyNames)
        {
            if (obj == null) { return $"{{null ({obj.GetType().Name})}}"; }
            var sb = new StringBuilder();
            sb.Append($"{{{obj.GetType().Name}");
            ToXamlProperties<T>(obj, sb, propertyNames);
            sb.Append("}");
            return sb.ToString();
        }
    }
}
