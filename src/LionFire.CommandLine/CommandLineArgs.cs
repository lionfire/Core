using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.CommandLine
{
    public class CommandLineArgs
    {
        public List<string> Args { get; set; } = new List<string>();
        public Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();


        public string Verb { get { if (Args.Count < 1) return null; return Args[0]; } }
        public IEnumerable<string> VerbArguments { get { if (Args.Count < 1) return null; return Args.Skip(1); } }

        public bool HasAnyArgs {
            get { return Options.Count > 0 || Args.Count > 0; }
        }
    }


    public static class CommandLineArgsExtensions
    {

        public static CommandLineArgs ParseCommandLineArgs(this string[] args)
        {
            var cla = new CommandLineArgs();

            cla.Args = new List<string>();

            foreach (var arg in args)
            {
                if (arg.StartsWith(CommandLineConfiguration.LongParameterPrefix) || arg.StartsWith(CommandLineConfiguration.ShortParameterPrefix))
                {
                    if (arg.Contains(CommandLineConfiguration.KeyValueSeparator))
                    {
                        var split = arg.Split(new char[] { CommandLineConfiguration.KeyValueSeparator }, 2);
                        cla.Options.Add(split[0], split[1]);
                        continue;
                    }
                    else
                    {
                        var argWithoutPrefix = arg;
                        if (argWithoutPrefix.StartsWith(CommandLineConfiguration.LongParameterPrefix))
                        {
                            argWithoutPrefix = argWithoutPrefix.Substring(CommandLineConfiguration.LongParameterPrefix.Length);
                        }
                        else //if (arg.StartsWith(CommandLineConfiguration.LongParameterPrefix))
                        {
                            argWithoutPrefix = argWithoutPrefix.Substring(CommandLineConfiguration.ShortParameterPrefix.Length);
                        }
                        cla.Options.Add(argWithoutPrefix, null);
                        continue;
                    }
                }
                cla.Args.Add(arg);
            }

            return cla;
        }
    }

    
}
