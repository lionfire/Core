using LionFire.CommandLine;
using LionFire.CommandLine.Arguments;

using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Services.Hosting
{

    public class HostCli
    {
        public void Run(string[] args)
        {
            LionEnvironment.StandardOutput.WriteLine("Test stdout");
            CommandLine.Dispatching.CliDispatcher.Dispatch(args, this);
        }

        static Dictionary<string, ServiceState> hostedServices = new Dictionary<string, ServiceState>();

        #region Command Line Options

        bool IsTestOptionEnabled = false;
        [CliOption(ShortForm = "t")]
        public void TestOption(string[] args)
        {
            Console.WriteLine("TESTOPTION");
            IsTestOptionEnabled = true;
        }

        [CliOption(ShortForm = "2")]
        public bool IsTestOption2Enabled { get; set; } = false;

        [CliOption(ShortForm = "3")]
        public string TestOption3 { get; set; } = "TestOption3Default";

        #endregion

        #region Command Line Verbs

        [CliVerb]
        public void Start(params string[] args)
        {
            //hostedServices.Add();

            var psi = new ProcessStartInfo("dotnet");
            psi.RedirectStandardOutput = true;
            var p = Process.Start(psi);
            while (!p.HasExited)
            {
                for (string line = p.StandardOutput.ReadLine(); line != null; line = p.StandardOutput.ReadLine())
                {
                    if (line != null)
                    {
                        LionEnvironment.StandardOutput.WriteLine("child: " + line);
                    }
                }
                Thread.Sleep(1000);
            }

            //var sh = new ServiceHost();
            //var sc = new ServiceConfig("LionFire.Services.Tests", "LionFire.Services.Tests.TestService", "TestParameter")
            //{
            //};
            //sh.Start(sc);
        }

        public void Stop(params string[] args)
        {
            if (args.Length < 0) { throw new ArgumentException("Requires a service name"); }

            var serviceState = hostedServices.TryGetValue(args[0]);

            Console.WriteLine("TODO: stop");
        }

        public static void Status(params string[] args)
        {
            Console.WriteLine("TODO: Status");
        }

        #endregion


    }
}
