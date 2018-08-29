using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Referencing
{
    public static class ReferenceConstants
    {
        public const string PortSeparator = ":";
        public const string PathSeparator = "/";
        public const char PathSeparatorChar = '/';

        public static Func<string, bool> IsLocalhost = host => LocalhostStrings.Contains(host);

        public static IEnumerable<string> LocalhostStrings
        {
            get
            {
                yield return "localhost";
                yield return "127.0.0.1";
                //"::1",
            }
        }
    }
}
