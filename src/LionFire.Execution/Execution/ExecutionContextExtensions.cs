using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Structures;
using LionFire.Execution.Initialization;

namespace LionFire.Execution
{


    public static class ExecutionContextExtensions
    {
        public async static Task<ExecutionContextInitializationState> Initialize(this ExecutionContext context)
        {
             await Singleton<ExecutionInitializer>.Instance.Initialize(context);
            return context.InitializationState;
        }

        public static async Task Start(this ExecutionContext executionContext)
        {
            await executionContext.Initialize();
            if (executionContext.InitializationState != ExecutionContextInitializationState.Initialized)
            {
                throw new Exception("Failed to initialize ExecutionContextState");
            }
            await executionContext.Controller.Start();
            return;
        }

        
    }
}
