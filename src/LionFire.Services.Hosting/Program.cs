using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LionFire.ExtensionMethods;

namespace LionFire.Services.Hosting
{
    public class CommandLineArgs
    {
        public List<string> Args { get; set; }
        public Dictionary<string, string> Options { get; set; }
    }
    public static class CommandLineUtils
    {
        public static bool AllowLongParameterNames = true;

        //public static char[] ParameterCharacters = new char[] { '/', '-' };
        public static char[] ParameterCharacters = new char[] { '-' };
        public static string LongParameterPrefix = "--";

        public static CommandLineArgs GetCommandLineArgs(this string[] args)
        {
            var cla = new CommandLineArgs();

            cla.Args = new List<string>(args);

            foreach (var arg in cla.Args.ToArray())
            {
                //foreach (var paramChar in ParameterCharacters)
                //{
                //    if (arg.StartsWith(paramChar))
                //    {
                //        var pa
                //    }
                //}
            }

            return cla;
        }
    }

    public class Program
    {

        public static void Main(string[] args)
        {
            
            var cla = args.GetCommandLineArgs();

            if (cla.Args.Count <= 0)
            {
                ShowUsage();
            }

            var commandArgs = cla.Args.Skip(1).ToArray();

            //var command = cla.Args[0];
            var command = "start";
            switch (command)
            {
                case "start":
                    Start(commandArgs);
                    break;
                case "stop":
                    Stop(commandArgs);
                    break;
                default:
                    ShowUsage();
                    break;
            }

            var sh = new ServiceHost();
            var sc = new ServiceConfig("LionFire.Services.Tests", "LionFire.Services.Tests.TestService", "TestParameter")
            {
            };
            sh.Start(sc);
        }

        static Dictionary<string, ServiceState> hostedServices = new Dictionary<string, ServiceState>();

        public static void Start(params string[] args)
        {
            //hostedServices.Add();

            var psi = new ProcessStartInfo("dotnet");
            psi.RedirectStandardOutput = true;
            var p = Process.Start(psi);
            while (!p.HasExited)
            {

                for (string line = p.StandardOutput.ReadLine(); line != null; line = p.StandardOutput.ReadLine()) {
                     
                    if (line != null)
                    {
                        Console.WriteLine("child: " + line);
                    }
                }
                Thread.Sleep(1000);
            }
        }
        public static void Stop(params string[] args)
        {
            if (args.Length < 0) { throw new ArgumentException("Requires a service name"); }

            var serviceState = hostedServices.TryGetValue(args[0]);
            



        }

        public static void Status(params string[] args)
        {

        }

        private static void ShowUsage()
        {
            Console.WriteLine("start/stop [serviceconfig] [service args] [-a ServiceArgsUrl]");
        }
    }


}
