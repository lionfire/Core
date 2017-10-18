using LionFire.Execution.Roslyn.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution.Roslyn.Scripting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(async()=> await RoslynScriptHost.TestScript()).Wait();
        }
    }
}
