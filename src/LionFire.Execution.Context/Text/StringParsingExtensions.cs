using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Text.Parsing
{
    public class StringParsing
    {
        public static string SplitOne(ref string str, char separator) // Move to LionFire.Text.Parsing?
        {
            if (!str.Contains(separator.ToString())) return null;

            var split = str.Split(new char[] { separator }, 2);

            str = split[1];
            return split[0];
        }
    }
}
