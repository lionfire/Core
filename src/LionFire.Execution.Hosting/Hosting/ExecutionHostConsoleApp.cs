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
using Microsoft.Extensions.Configuration;

namespace LionFire.Execution.Hosting
{

    public class ExecutionHostConsoleApp : ConsoleApp
    {
        ProcessExecutionHost host { get { return ProcessExecutionHost.Instance; } }

        private void ConfigTest()
        {
            var builder = new ConfigurationBuilder()
            //.AddJsonFile("ExecutionSettings.json")
            .AddInMemoryCollection(new[]
            {
                new KeyValuePair<string,string>("the-key", "the-value"),
            });

            var configuration = builder.Build();


            var configValue = configuration["the-key"];
            Console.WriteLine($"The value for 'the-key' is '{configValue}'");
        }

        #region Command Line Options

        #region TEST

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

        #endregion

        #region Command Line Verbs

        [CliVerb]
        public void Start(params string[] args)
        {
            var ec = new ExecutionContext(args.Aggregate((x, y) => x + " " + y));
            ec.Initialize().Wait();
            ec.Start().Wait();
            ProcessExecutionHost.Instance.WaitForAllExecutionsToComplete();
        }

        [CliVerb]
        public void Stop(params string[] args)
        {
            if (args.Length < 0) { throw new ArgumentException("Requires a service name"); }

            //var serviceState = host.hostedExecutions.TryGetValue(args[0]);

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
