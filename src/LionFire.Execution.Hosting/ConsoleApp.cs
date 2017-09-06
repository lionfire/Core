using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.CommandLine.Dispatching;
using System.Reflection;

namespace LionFire.Applications.CommandLine
{
    // MOVE to LionFire.Application.Console

    public class ConsoleApp
    {
        public void Run(string[] args)
        {
            // TODO: Add detailed info command
            CliDispatcher.DefaultOptions.AdditionalHandlerTypes.Add(typeof(LionFire.Environment.LionEnvironmentCliHandlers));

            //var a = Assembly.Load(new AssemblyName("LionFire.Environment.CommandLine"));
            //Console.WriteLine("ASSEMbLY: " + a.ToString());
            //var t = a.GetType("LionFire.Environment.LionEnvironmentCliHandlers");
            //Console.WriteLine("TYPE: " + t.ToString());

            //Console.WriteLine(typeof(LionFire.Environment.LionEnvironmentCliHandlers).AssemblyQualifiedName);
            CliDispatcher.Dispatch(args, this);
            LionFireEnvironment.StandardOutput?.Flush(); // REVIEW
        }
    }
}
