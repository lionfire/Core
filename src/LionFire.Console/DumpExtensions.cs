using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ConsoleUtils
{
    public static class DumpExtensions
    {
        public static string DumpHeader(this string header, string underline = "=", int marginBottom = 1) {
            var sb = new StringBuilder();
            sb.AppendLine(header);

            if (underline.Length > 0) {
                for (int i = header.Length; i > 0; i--) sb.Append(underline);
            }
            sb.AppendLine();

            for (int i = marginBottom; i > 0; i--) sb.AppendLine();
            return sb.ToString();
        }
        public static string DumpProperties(this object obj, string header = null, bool ignoreDefaultValues = true, string bullet = " - ", string equals = " = ", string underline = "-") {
            var sb = new StringBuilder();
            if (header != null) {
                sb.Append(DumpHeader(header, underline: underline, marginBottom: 0));
                //sb.AppendLine(header);
                //for (int i = 0; i < header.Length; i++) sb.Append("-");
                //sb.AppendLine();
            }

            foreach (var pi in obj.GetType().GetProperties()) {
                var val = pi.GetValue(obj);
                if (ignoreDefaultValues) {
                    object defaultVal = pi.PropertyType.IsValueType ? Activator.CreateInstance(pi.PropertyType) : null;
                    if (val == defaultVal) continue;
                }
                sb.Append(bullet);
                sb.Append(pi.Name);
                sb.Append(equals);
                sb.Append(pi.GetValue(obj));
                sb.AppendLine();
            }
            if (header != null) {
                sb.AppendLine(); // Bottom margin
            }
            return sb.ToString();
        }

        public static string DumpList<T>(this IEnumerable<T> list, string header = null, Func<T, string> displayFunc = null) {
            var sb = new StringBuilder();
            if (header != null) {
                sb.AppendLine(header);
                for (int i = 0; i < header.Length; i++) sb.Append("-");
                sb.AppendLine();
            }

            foreach (var pi in list) {
                sb.Append(" - ");
                sb.AppendLine(displayFunc != null ? displayFunc(pi) : pi.ToString());
            }
            if (header != null) {
                sb.AppendLine(); // Bottom margin
            }
            return sb.ToString();
        }
    }
}
