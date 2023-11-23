using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string InsertSpaceBeforeCaps(this string input)
        {
            var sb = new StringBuilder();
            foreach (var c in input)
            {
                if (sb.Length > 0 && char.IsUpper(c)) sb.Append(" ");
                sb.Append(c);
            }
            return sb.ToString();
        }
        public static string TryRemoveFromEnd(this string str, string stringToRemove)
        {
            if (str.EndsWith(stringToRemove))
            {
                str = str.Substring(0, str.Length - stringToRemove.Length);
            }
            return str;
        }
        public static string TryRemoveFromStart(this string s, string stringToRemove)
        {
            if (s.StartsWith(stringToRemove)) return s.Substring(stringToRemove.Length);
            return s;
        }
        public static string TryRemovePrefixFromStart(this string s, string stringToRemove)
        {
            if (s.HasPrefix(stringToRemove)) return s.Substring(stringToRemove.Length);
            return s;
        }

        public static bool HasPrefix(this string s, string prefix) => s.StartsWith(prefix) && s.Length > prefix.Length && char.IsUpper(s[prefix.Length]);

        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
    }
}
