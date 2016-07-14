using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using LionFire.Structures;
using LionFire.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Execution.Initialization
{
    public class ExecutionInitializer
    {
        HashSet<string> LocalhostNames = new HashSet<string>()
        {
            "localhost",
            ".",
            "127.0.0.1",
        };
       

        public async Task<bool> Initialize(ExecutionContext context)
        {
            context.InitializationState = ExecutionContextInitializationState.MissingEverything;

            #region Get Config Object

            var c = context.Config;
            if (c == null)
            {
                context.InitializationState = ExecutionContextInitializationState.Uninitialized;
                return false;
            }

            #endregion

            #region Load Config

            // TODO: Resolve default config (which may fill in some extra params)
            if (c.ConfigName != null)
            {
                var configDir = @"E:\src\temp\ExecutionConfigs";
                var suffix = ".json";
                var configPath = Path.Combine(configDir, context.Config.ConfigName + suffix);
                try
                {
                    c.ImportFromFile(configPath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to load configuration '{c.ConfigName}' from file: {configPath}", ex);
                }
            }

            #endregion

            #region IsLocal

            bool isLocal = true;
            if (String.IsNullOrWhiteSpace(context.Config.ExecutionLocation))
            {
                // FUTURE: Unless default config for specified EC is to run somewhere else
                isLocal = true;
            }
            else
            {
                isLocal = false;
                if (LocalhostNames.Contains (c.ExecutionLocation.ToLowerInvariant()))
                {
                    isLocal = true;
                }
            }

            #endregion

            var sp = ManualSingleton<IServiceCollection>.Instance.BuildServiceProvider(); // TODO: centralize and do this only once?


            var executionLocationType = c.ExecutionLocationType;
            if (executionLocationType == ExecutionLocationType.OtherMachine && isLocal)
            {
                executionLocationType = ExecutionLocationType.LocalMachine;
            }

            switch (executionLocationType)
            {
                case ExecutionLocationType.InProcess:
                    context.Controller = new ObjectController() { ExecutionContext = context };
                    context.InitializationState &= ~ExecutionContextInitializationState.MissingHost;
                    break;
                case ExecutionLocationType.CurrentUser:
                    break;
                case ExecutionLocationType.LocalMachine:
                    break;
                case ExecutionLocationType.OtherMachine:
                    context.Controller = new RemoteExecutionControllerProxy();
                    break;
                case ExecutionLocationType.Hive:
                    context.Controller = new RemoteExecutionControllerProxy();
                    break;
                case ExecutionLocationType.Global:
                    break;
                default:
                    break;
            }

            if (context.Controller != null)
            {
                context.InitializationState &= ~ExecutionContextInitializationState.MissingExecutor;
                
            }
            else {
                return false;
            }

            var result = await context.Controller.Initialize();

            //foreach (var s in sp.GetServices<IExecutionControllerProvider>())
            //{
            //    if (s.TryAttachController(context)) break;
            //}

            return context.InitializationState == ExecutionContextInitializationState.Initialized;
        }

        //private async Task<bool> InitializeInProcess(ExecutionContext context)
        //{
        //    if (!(await context.TryResolveServiceType()))
        //    {
        //        Console.WriteLine("ERROR: Failed to resolve: " + sc.Arg);
        //        return Task.FromResult(false);
        //    }
        //}

    }
}
