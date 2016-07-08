using LionFire.Applications.CommandLine;
using LionFire.CommandLine;
using LionFire.CommandLine.Arguments;

using LionFire.ExtensionMethods;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace LionFire.Services.Hosting
{

    public class ServiceHostCli : CliApp
    {
        ServiceHost host = new ServiceHost();

        #region Command Line Options

        bool IsTestOptionEnabled = false;
        [CliOption(ShortForm = 't')]
        public void TestOption(string[] args)
        {
            Console.WriteLine("TESTOPTION");
            IsTestOptionEnabled = true;
        }

        [CliOption(ShortForm = '2')]
        public bool IsTestOption2Enabled { get; set; } = false;

        [CliOption(ShortForm = '3')]
        public string TestOption3 { get; set; } = "TestOption3Default";

        #endregion

        #region Command Line Verbs

        [CliVerb]
        public void Start(params string[] args)
        {
            host.Start(args);       
        }

        

        [CliVerb]
        public void Stop(params string[] args)
        {
            if (args.Length < 0) { throw new ArgumentException("Requires a service name"); }

            var serviceState = host.hostedServices.TryGetValue(args[0]);

            Console.WriteLine("TODO: stop");
        }

        [CliVerb]
        public static void Status(params string[] args)
        {
            Console.WriteLine("TODO: Status");
        }

        [CliVerb]
        public static void Usage()
        {
            Console.WriteLine("SAMPLE USAGE");
        }

        [CliVerb]
        public void Info()
        {
            Console.WriteLine("IsTestOptionEnabled: " + IsTestOptionEnabled);
        }

        #endregion


    }
}
