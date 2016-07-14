using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;


namespace LionFire.Execution.Roslyn.Scripting
{


    public class RoslynScriptExecutionProvider : IExecutionControllerProvider
    {
        public bool TryAttachController(ExecutionContext context)
        {
            context.Controller = new RoslynScriptExecutionController(context);
            return false;
        }

        public ImmutableArray<string> RuntimeNames = new ImmutableArray<string> { "roslyn", "roslyn-script" };

        public bool IsCompatible(ExecutionContext context)
        {
            var c = context.Config;

            if (context.Config == null) throw new ArgumentNullException(nameof(ExecutionContext.Config));
            if (context.Config.Runtime != null)
            {
                if (RuntimeNames.Contains(context.Config.Runtime))
                {
                    return false;
                }
            }

            //if(c.Mime

            return true;
        }
    }

    public static class RoslynScriptExecutionProviderExtensions
    {
        public static IServiceCollection AddRoslynScripting(this IServiceCollection sc)
        {
            sc.AddSingleton(typeof(IExecutionControllerProvider), typeof(RoslynScriptExecutionProvider));
            return sc;
        }
    }
}
