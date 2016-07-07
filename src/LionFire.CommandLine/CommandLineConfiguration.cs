using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.CommandLine
{
    public static class CommandLineConfiguration
    {
        #region Constants

        public static string ShortParameterPrefix = "-";
        public static string LongParameterPrefix = "--";

        public static char KeyValueSeparator = ':';

        #endregion

        public static bool AllowLongParameterNames = true;
    }
}
