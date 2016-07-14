using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Execution.Hosting;
using LionFire.Execution.Configuration;
using LionFire.Execution.Initialization;

namespace LionFire.Execution
{

    /// <summary>
    /// Uses an in-process ProcessExecutionHost
    /// </summary>
    public class ObjectController : IExecutionController
    {
        #region Configuration

        ProcessExecutionHost Host { get; set; } = ProcessExecutionHost.Instance;

        #endregion

        #region Relationships

        [SetOnce]
        public ExecutionContext ExecutionContext { get; set; }

        #endregion


        public async Task<bool> Initialize()
        {
            var c = ExecutionContext.Config;

            var result = await c.ResolveType();
            if (c.Type != null)
            {
                ExecutionContext.InitializationState &= ~(ExecutionContextInitializationState.MissingCode);
            }

            var defaultConstructor = ExecutionContext.Config.Type.GetConstructor(new Type[] { });

            if (defaultConstructor != null)
            {
                ExecutionContext.InitializationState &= ~(ExecutionContextInitializationState.MissingConstructor);
            }

            if (typeof(IExecutable).GetTypeInfo().IsAssignableFrom(c.Type))
            {
                ExecutionContext.Status = ExecutionHostState.Initializing;
                ExecutionContext.ExecutionObject = Activator.CreateInstance(c.Type);
                ExecutionContext.Status = ExecutionHostState.Initialized;
            }
            else
            {
                // Don't create ExecutionObject until Start
            }

            if ((ExecutionContext.InitializationState & ExecutionContextInitializationState.MissingEverything) == ExecutionContextInitializationState.None)
            {
                ExecutionContext.Status = ExecutionHostState.Initialized;
                ExecutionContext.InitializationState = ExecutionContextInitializationState.Initialized;
            }

            return ExecutionContext.Status == ExecutionHostState.Initialized;

            //var result = await c.ResolveObject();
            //if (c.Object != null)
            //{
            //    ExecutionContext.InitializationState &= ~(ExecutionContextInitializationState.MissingCode | ExecutionContextInitializationState.MissingExecutor | ExecutionContextInitializationState.MissingHost);
            //}
            //return result;

            //    if (!(await sc.TryResolveServiceType()))
            //    {
            //        Console.WriteLine("ERROR: Failed to resolve: " + sc.Arg);
            //        return;
            //    }
        }

        public bool IsIExecutable {
            get {
                var type = ExecutionContext.Config.Type;
                return type != null && typeof(IExecutable).GetTypeInfo().IsAssignableFrom(type);
            }
        }
        public bool IsIDisposable {
            get {
                var type = ExecutionContext.Config.Type;
                return type != null && typeof(IDisposable).GetTypeInfo().IsAssignableFrom(type);
            }
        }

        public async Task Start(/*params string[] args*/)
        {
            Host.Register(ExecutionContext);

            if (ExecutionContext.Status != ExecutionHostState.Initialized)
            {
                throw new Exception("Not initialized");
            }

            ExecutionContext.Status = ExecutionHostState.Starting;

            if (IsIExecutable)
            {
                IExecutable execObj = ExecutionContext.ExecutionObject as IExecutable;
                await execObj.Start();
                ExecutionContext.Status = ExecutionHostState.Running;
            }
            else if (IsIDisposable)
            {
                ExecutionContext.RunTask = Task.Run(() =>
                {
                    ExecutionContext.ExecutionObject = Activator.CreateInstance(ExecutionContext.Config.Type);
                    ExecutionContext.Status = ExecutionHostState.Running;
                });
            }
            else
            {
                ExecutionContext.RunTask = Task.Run(() =>
                {
                    ExecutionContext.Status = ExecutionHostState.Running;
                    ExecutionContext.ExecutionObject = Activator.CreateInstance(ExecutionContext.Config.Type);
                    ExecutionContext.Status = ExecutionHostState.Finished;
                });
            }
        }

        public async Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren)
        {
            
            ExecutionContext.Status = ExecutionHostState.Stopping;
            if (IsIExecutable)
            {
                IExecutable execObj = ExecutionContext.ExecutionObject as IExecutable;
                await execObj.Stop(mode, options);
                ExecutionContext.Status = ExecutionHostState.Stopped;
            }
            else if (IsIDisposable)
            {
                await Task.Run(() =>
                {
                    ExecutionContext.ExecutionObject = Activator.CreateInstance(ExecutionContext.Config.Type);
                    ExecutionContext.Status = ExecutionHostState.Running;
                });
                
            }
            else
            {
                await Task.Run(() =>
                {
                    ExecutionContext.Status = ExecutionHostState.Running;
                    ExecutionContext.ExecutionObject = Activator.CreateInstance(ExecutionContext.Config.Type);
                    ExecutionContext.Status = ExecutionHostState.Finished;
                });
            }
        }
    }
}



//private async Task StartInProcess()
//{
//    var sc = ExecutionContext;
//    startTasks.Add(sc.Guid, Task.Factory.StartNew(() =>
//    {
//        try
//        {
//            var serviceState = hostedExecutions.TryGetValue(sc.Key);
//            if (serviceState == null)
//            {
//                serviceState = new ServiceState(sc);
//                hostedExecutions.Add(sc.Key, serviceState);
//            }
//            if (serviceState.Status.IsActive()) return;


//        }
//        finally
//        {
//            startTasks.Remove(sc.Key);
//        }
//    }));
//}

        //private void StopInProcess(ServiceConfig sc)
        //{
        //    stopTasks.Add(sc.Key, Task.Factory.StartNew(() =>
        //    {
        //        try
        //        {
        //            var serviceState = hostedExecutions.TryGetValue(sc.Key);
        //            if (serviceState == null)
        //            {
        //                return;
        //            }

        //            if (serviceState.Service != null)
        //            {
        //                serviceState.Status = ExecutionState.Stopping;
        //                serviceState.Service.Stop(StopMode.GracefulShutdown, StopOptions.StopChildren);
        //                serviceState.Status = ExecutionState.Stopped;
        //            }
        //            else if (serviceState.IsDisposable)
        //            {
        //                // TODO: wait for non-intermediate state
        //                serviceState.Status = ExecutionState.Stopping;
        //                ((IDisposable)serviceState.ServiceObject).Dispose();
        //                serviceState.ServiceObject = null;
        //                serviceState.Status = ExecutionState.Uninitialized;
        //            }
        //            else
        //            {
        //                serviceState.Status = ExecutionState.Stopped;
        //            }
        //        }
        //        finally
        //        {
        //            stopTasks.Remove(sc.Key);
        //        }
        //    }));
        //}


    

