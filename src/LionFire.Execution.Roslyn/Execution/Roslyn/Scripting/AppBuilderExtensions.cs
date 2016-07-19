using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Execution.Roslyn.Scripting
{
    public static class AppBuilderExtensions
    {
        public static IServiceCollection AddRoslynScripting(this IServiceCollection sc) // TODO: IAppBuilder of some sort
        {
            sc.AddSingleton(typeof(IExecutionControllerProvider), typeof(RoslynScriptExecutionProvider));
            return sc;
        }
    }
}
