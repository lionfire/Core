using LionFire.CommandLine.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Environment
{

    //        if (options.ProvideVersion && !argDefs.Where(def => def.LongForm.ToLowerInvariant() == "version").Any())
    //        {
    //            argDefs.Add(new ArgumentDefinition
    //            {
    //                LongForm = "version",
    //                ShortForm = "V",
    //                UsesOptionPrefix = true,
    //                IsExclusive = true,
    //                Description = "Prints the program version information.",
    //                Handler
    //});
    //        }

    public static class LionEnvironmentCliHandlers
    {
        [CliDescription("Prints the version info for the program.")]
        public static void Version()
        {
            var sb = new StringBuilder();
            sb.Append(LionFireEnvironment.ProgramName);
            sb.Append(" ");
            sb.Append(LionFireEnvironment.MainAppInfo.ProgramVersion);
            sb.AppendLine();

            Console.WriteLine(sb.ToString());
        }
    }
}
