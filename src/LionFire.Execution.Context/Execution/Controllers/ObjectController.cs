using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Execution.Hosting;
using LionFire.Execution.Configuration;
using LionFire.Execution.Initialization;
using System.Reactive.Subjects;
using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using LionFire.Execution.Executables;
using System.Threading;

namespace LionFire.Execution
{

    /// <summary>
    /// Uses an in-process ProcessExecutionHost
    /// </summary>
    public class ObjectController : ExecutableExBase, IExecutionController
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

            var result = await c.ResolveType().ConfigureAwait(false);
            if (c.Type != null)
            {
                ExecutionContext.InitializationState &= ~(ExecutionContextInitializationState.MissingCode);
            }

            var defaultConstructor = ExecutionContext.Config.Type.GetConstructor(new Type[] { });

            if (defaultConstructor != null)
            {
                ExecutionContext.InitializationState &= ~(ExecutionContextInitializationState.MissingConstructor);
            }

            if (typeof(IExecutableEx).GetTypeInfo().IsAssignableFrom(c.Type))
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

            //var result = await c.ResolveObject().ConfigureAwait(false);
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

        public bool IsStartable {
            get {
                var type = ExecutionContext.Config.Type;
                return type != null && typeof(IStartable).GetTypeInfo().IsAssignableFrom(type);
            }
        }
        public bool IsIDisposable {
            get {
                var type = ExecutionContext.Config.Type;
                return type != null && typeof(IDisposable).GetTypeInfo().IsAssignableFrom(type);
            }
        }


        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            Host.Register(ExecutionContext);

            if (ExecutionContext.Status != ExecutionHostState.Initialized)
            {
                throw new Exception("Not initialized");
            }

            ExecutionContext.Status = ExecutionHostState.Starting;

            if (IsStartable)
            {
                var execObj = ExecutionContext.ExecutionObject as IStartable;
                await execObj.StartAsync().ConfigureAwait(false);
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

        public Task StopAsync(CancellationToken cancellationToken = default) => Stop(cancellationToken: cancellationToken);

        public async Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren, CancellationToken cancellationToken = default)
        {
            ExecutionContext.Status = ExecutionHostState.Stopping;
            if (ExecutionContext.ExecutionObject as IStoppableEx != null)
            {
                var execObj = ExecutionContext.ExecutionObject as IStoppableEx;
                await execObj.Stop(mode, options).ConfigureAwait(false);
                ExecutionContext.Status = ExecutionHostState.Stopped;
            }
            else if (ExecutionContext.ExecutionObject as IStoppable != null)
            {
                var execObj = ExecutionContext.ExecutionObject as IStoppable;
                await execObj.StopAsync().ConfigureAwait(false);
                ExecutionContext.Status = ExecutionHostState.Stopped;
            }
            else if (IsIDisposable)
            {
                await Task.Run(() =>
                {
                    ExecutionContext.ExecutionObject = Activator.CreateInstance(ExecutionContext.Config.Type);
                    ExecutionContext.Status = ExecutionHostState.Running;
                }).ConfigureAwait(false);
                
            }
            else
            {
                await Task.Run(() =>
                {
                    ExecutionContext.Status = ExecutionHostState.Running;
                    ExecutionContext.ExecutionObject = Activator.CreateInstance(ExecutionContext.Config.Type);
                    ExecutionContext.Status = ExecutionHostState.Finished;
                }).ConfigureAwait(false);
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


    

