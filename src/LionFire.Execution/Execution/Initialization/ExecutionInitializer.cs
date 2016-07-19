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

            var config = context.Config;
            if (config == null)
            {
                context.InitializationState = ExecutionContextInitializationState.Uninitialized;
                return false;
            }

            #endregion

            #region Load Config

            // TODO: Resolve default config (which may fill in some extra params)
            if (config.ConfigName != null)
            {
                var configDir = @"E:\src\temp\ExecutionConfigs"; // HARDPATH TEMP
                var suffix = ".json";
                var configPath = Path.Combine(configDir, context.Config.ConfigName + suffix);
                try
                {
                    config.ImportFromFile(configPath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to load configuration '{config.ConfigName}' from file: {configPath}", ex);
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
                if (LocalhostNames.Contains(config.ExecutionLocation.ToLowerInvariant()))
                {
                    isLocal = true;
                }
            }

            #endregion

            #region SourceContent

            bool result = await Singleton<ExecutionConfigResolver>.Instance.ResolveSourceContent(config);
            if (!result) return false;

            #endregion

            var sp = ManualSingleton<IServiceCollection>.Instance.BuildServiceProvider(); // TODO: centralize and do this only once?

            var executionLocationType = config.ExecutionLocationType;
            if (executionLocationType == ExecutionLocationType.OtherMachine && isLocal)
            {
                executionLocationType = ExecutionLocationType.LocalMachine;
            }

            bool gotController = false;
            bool controllerInitSucceeded = false;
            bool controllerInitializeAttempted = false;
            switch (executionLocationType)
            {
                case ExecutionLocationType.InProcess:


                    if (context.Config.Type != null)
                    {
                        context.Controller = new ObjectController() { ExecutionContext = context }; // Move to AddExecutionDefaults extension method on app builder
                        gotController = true;
                    }
                    else
                    {
                        foreach (var svc in sp.GetServices<IExecutionControllerProvider>())
                        {
                            controllerInitializeAttempted = false;
                            gotController |= svc.TryAttachController(context);

                            if (gotController)
                            {
                                try
                                {
                                    controllerInitializeAttempted = true;
                                    var controllerInitResult = await context.Controller.Initialize();
                                    if (controllerInitResult)
                                    {
                                        gotController = true;
                                        controllerInitSucceeded = true;
                                    }
                                }
                                catch (ExecutionInitializationException)
                                {
                                    // Got the right controller but it failed on the input.
                                    gotController = true;
                                }
                            }

                            if (gotController) break;
                        }
                    }
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
                    throw new NotImplementedException();
            }

            #region Init controller if we haven't yet
            
            if (!controllerInitializeAttempted && context.Controller != null)
            {
                var controllerInitResult = await context.Controller.Initialize();
                if (controllerInitResult)
                {
                    gotController = true;
                    controllerInitSucceeded = true;
                }
            }

            #endregion

            #region Update InitializationState based on what happened here

            if (gotController)
            {
                context.InitializationState &= ~(ExecutionContextInitializationState.MissingHost | ExecutionContextInitializationState.MissingController);
            }
            if (controllerInitSucceeded)
            {
                context.InitializationState &= ~(ExecutionContextInitializationState.MissingControllerInitialization);
            }
            if (context.Controller != null)
            {
                context.InitializationState &= ~ExecutionContextInitializationState.MissingController;
            }
            else
            {
                return false;
            }
            if (context.InitializationState == ExecutionContextInitializationState.None)
            {
                context.InitializationState = ExecutionContextInitializationState.Initialized;
            }

            #endregion

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
