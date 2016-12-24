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
    }
}
