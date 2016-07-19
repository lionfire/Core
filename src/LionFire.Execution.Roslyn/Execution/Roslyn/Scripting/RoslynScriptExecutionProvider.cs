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
            if (!IsCompatible(context)) return false;

            context.Controller = new RoslynScriptExecutionController(context);
            return true;
        }

        public readonly List<string> RuntimeNames = new List<string> { "roslyn", "roslyn-script" };

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

            if (context.Config.SourceContent == null)
            {
                return false;
            }

            return true;
        }
    }

    
}
