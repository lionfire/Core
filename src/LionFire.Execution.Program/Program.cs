using System;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Structures;
using LionFire.Execution.Roslyn.Scripting;


namespace LionFire.Execution.Hosting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ManualSingleton<IServiceCollection>.Instance = new ServiceCollection();

            ManualSingleton<IServiceCollection>.Instance
                .AddRoslynScripting()
                ;

            new ExecutionHostConsoleApp().Run(args);
        }
    }
}
