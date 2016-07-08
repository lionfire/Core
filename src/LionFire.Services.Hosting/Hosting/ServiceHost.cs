using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Services.Hosting
{
    public class ServiceHost
    {
        #region State

        internal Dictionary<string, ServiceState> hostedServices = new Dictionary<string, ServiceState>();
        Dictionary<string, Task> startTasks = new Dictionary<string, Task>();
        Dictionary<string, Task> runTasks = new Dictionary<string, Task>();
        Dictionary<string, Task> stopTasks = new Dictionary<string, Task>();

        #endregion

        #region In Process

        private async Task StartInProcess(ServiceConfig sc)
        {
            if (!(await sc.TryResolveServiceType()))
            {
                Console.WriteLine("ERROR: Failed to resolve: " + sc.Arg);
                return;
            }

            startTasks.Add(sc.Key, Task.Factory.StartNew(() =>
            {
                try
                {
                    var serviceState = hostedServices.TryGetValue(sc.Key);
                    if (serviceState == null)
                    {
                        serviceState = new ServiceState(sc);
                        hostedServices.Add(sc.Key, serviceState);
                    }
                    if (serviceState.IsActive) return;

                    if (typeof(IService).IsAssignableFrom(sc.ServiceType))
                    {
                        serviceState.Status = ServiceStatus.Initializing;
                        serviceState.ServiceObject = Activator.CreateInstance(sc.ServiceType);
                        serviceState.Status = ServiceStatus.Initialized;

                        serviceState.Status = ServiceStatus.Starting;
                        serviceState.Service.Start();
                        serviceState.Status = ServiceStatus.Started;
                    }
                    else
                    {
                        serviceState.Status = ServiceStatus.Starting;
                        serviceState.ServiceObject = Activator.CreateInstance(sc.ServiceType);
                        serviceState.Status = ServiceStatus.Started;
                    }
                }
                finally
                {
                    startTasks.Remove(sc.Key);
                }
            }));
        }

        private void StopInProcess(ServiceConfig sc)
        {
            stopTasks.Add(sc.Key, Task.Factory.StartNew(() =>
            {
                try
                {
                    var serviceState = hostedServices.TryGetValue(sc.Key);
                    if (serviceState == null)
                    {
                        return;
                    }

                    if (serviceState.Service != null)
                    {
                        serviceState.Status = ServiceStatus.Stopping;
                        serviceState.Service.Stop(StopMode.GracefulShutdown, StopOptions.StopChildren);
                        serviceState.Status = ServiceStatus.Stopped;
                    }
                    else if (serviceState.IsDisposable)
                    {
                        // TODO: wait for non-intermediate state
                        serviceState.Status = ServiceStatus.Stopping;
                        ((IDisposable)serviceState.ServiceObject).Dispose();
                        serviceState.ServiceObject = null;
                        serviceState.Status = ServiceStatus.Uninitialized;
                    }
                    else
                    {
                        serviceState.Status = ServiceStatus.Stopped;
                    }
                }
                finally
                {
                    stopTasks.Remove(sc.Key);
                }
            }));
        }

        #endregion


        public bool IsServiceRunning(ServiceConfig sc)
        {
            switch (sc.HostingLocationType)
            {
                case HostingLocationType.InProcess:
                    // Check in-process
                    var serviceState = hostedServices.TryGetValue(sc.Key ?? sc.Arg); // TODO: how to resolve key before resolving type?
                    return serviceState != null && serviceState.IsActive;
                //case HostingLocationType.CurrentUser:
                //    break;
                //case HostingLocationType.LocalMachine:
                //    break;
                //case HostingLocationType.Hive:
                //    break;
                //case HostingLocationType.Global:
                //    break;
                default:
                    throw new NotImplementedException();
            }
        }


        public void Start(params string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("TODO: Start default configuration");
                return;
            }

            var conf = new ServiceConfig(args[1]);

            if (IsServiceRunning(conf))
            {
                Console.WriteLine("[start] Already running: " + conf);
                return;
            }

            List<Task> startupTasks = new List<Task>();
            switch (conf.HostingLocationType)
            {
                case HostingLocationType.InProcess:
                    startupTasks.Add(StartInProcess(conf));
                    break;
                //case HostingLocationType.CurrentUser:
                //    break;
                //case HostingLocationType.LocalMachine:
                //    break;
                //case HostingLocationType.Hive:
                //    break;
                //case HostingLocationType.Global:
                //    break;
                default:
                    throw new NotImplementedException();
            }
            //hostedServices.Add();

            //var psi = new ProcessStartInfo("dotnet");
            //psi.RedirectStandardOutput = true;
            //var p = Process.Start(psi);
            //while (!p.HasExited)
            //{
            //    for (string line = p.StandardOutput.ReadLine(); line != null; line = p.StandardOutput.ReadLine())
            //    {
            //        if (line != null)
            //        {
            //            LionEnvironment.StandardOutput.WriteLine("child: " + line);
            //        }
            //    }
            //    Thread.Sleep(1000);
            //}

            //var sh = new ServiceHost();
            //var sc = new ServiceConfig("LionFire.Services.Tests", "LionFire.Services.Tests.TestService", "TestParameter")
            //{
            //};
            //sh.Start(sc);

            WaitForTasksToComplete(startupTasks.ToArray());
            Console.WriteLine("Everything finished.");
            Console.ReadKey();
        }

        private void WaitForTasksToComplete(params Task[] tasks)
        {
            foreach (var task in tasks)
            {
                if (!task.IsCompleted)
                {
                    Console.WriteLine("Waiting for a startup task to complete.");
                }
                task.Wait();
            }

            bool gotTask;
            do
            {
                gotTask = false;
                foreach (var kvp in startTasks.Concat(stopTasks).Concat(runTasks).ToArray())
                {
                    gotTask = true;
                    Console.WriteLine("Waiting for " + kvp.Key + " to complete.");
                    kvp.Value.Wait();
                }
            } while (gotTask);

        }

    }

}
