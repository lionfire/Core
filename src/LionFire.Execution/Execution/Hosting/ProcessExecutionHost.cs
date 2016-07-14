using LionFire.Execution;
using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Structures;

namespace LionFire.Execution.Hosting
{
    public class ProcessExecutionHost
    {
        public static ProcessExecutionHost Instance { get { return Singleton<ProcessExecutionHost>.Instance; } }
        

        #region State

        public Dictionary<Guid, ExecutionContext> hostedExecutions = new Dictionary<Guid, ExecutionContext>();
        Dictionary<string, Task> startTasks = new Dictionary<string, Task>();
        Dictionary<string, Task> runTasks = new Dictionary<string, Task>();
        Dictionary<string, Task> stopTasks = new Dictionary<string, Task>();

        #endregion

        public void Register(ExecutionContext ec)
        {
            if (!hostedExecutions.ContainsKey(ec.Guid))
            {
                this.hostedExecutions.Add(ec.Guid, ec);
            }
        }
        public void Unregister(ExecutionContext ec)
        {
            this.hostedExecutions.Remove(ec.Guid);
        }

        public void WaitForAll()
        {
            foreach (var h in hostedExecutions.Values)
            {
                if (h.RunTask != null)
                {
                    h.RunTask.Wait();
                }
            }
        }


        #region In Process

        #endregion


        //public bool IsRunning(ExecutionContext context) TODO
        //{
        //    switch (context.ExecutionLocationType)
        //    {
        //        case ExecutionLocationType.InProcess:
        //            // Check in-process
        //            var serviceState = hostedExecutions.TryGetValue(context.Key ?? context.Arg); // TODO: how to resolve key before resolving type?
        //            return serviceState != null && serviceState.Status.IsActive();
        //        //case ExecutionLocationType.CurrentUser:
        //        //    break;
        //        //case ExecutionLocationType.LocalMachine:
        //        //    break;
        //        //case ExecutionLocationType.Hive:
        //        //    break;
        //        //case ExecutionLocationType.Global:
        //        //    break;
        //        default:
        //            throw new NotImplementedException();
        //    }
        //}



        //public void StartOld(params string[] args)
        //{

        //    if (args.Length == 0)
        //    {
        //        Console.WriteLine("TODO: Start default configuration");
        //        return;
        //    }

        //    var conf = new ServiceConfig(args[1]);

        //    if (IsRunning(conf))
        //    {
        //        Console.WriteLine("[start] Already running: " + conf);
        //        return;
        //    }

        //    List<Task> startupTasks = new List<Task>();
        //    switch (conf.ExecutionLocationType)
        //    {
        //        case ExecutionLocationType.InProcess:
        //            startupTasks.Add(StartInProcess(conf));
        //            break;
        //        //case ExecutionLocationType.CurrentUser:
        //        //    break;
        //        //case ExecutionLocationType.LocalMachine:
        //        //    break;
        //        //case ExecutionLocationType.Hive:
        //        //    break;
        //        //case ExecutionLocationType.Global:
        //        //    break;
        //        default:
        //            throw new NotImplementedException();
        //    }
        //    //hostedExecutions.Add();

        //    //var psi = new ProcessStartInfo("dotnet");
        //    //psi.RedirectStandardOutput = true;
        //    //var p = Process.Start(psi);
        //    //while (!p.HasExited)
        //    //{
        //    //    for (string line = p.StandardOutput.ReadLine(); line != null; line = p.StandardOutput.ReadLine())
        //    //    {
        //    //        if (line != null)
        //    //        {
        //    //            LionEnvironment.StandardOutput.WriteLine("child: " + line);
        //    //        }
        //    //    }
        //    //    Thread.Sleep(1000);
        //    //}

        //    //var sh = new ServiceHost();
        //    //var sc = new ServiceConfig("LionFire.Services.Tests", "LionFire.Services.Tests.TestService", "TestParameter")
        //    //{
        //    //};
        //    //sh.Start(sc);

        //    WaitForTasksToComplete(startupTasks.ToArray());
        //    Console.WriteLine("Everything finished.");
        //    Console.ReadKey();
        //}

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
