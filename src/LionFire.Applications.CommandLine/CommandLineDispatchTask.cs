using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.CommandLine;
using System.Threading;
using LionFire.CommandLine.Dispatching;

namespace LionFire.Applications.Hosting
{
    public class CommandLineDispatchTask : IAppTask
    {
        #region Parameters

        public string[] Args { get; set; }
        public object DispatcherObject { get; set; }
        public Type DispatcherType { get; set; }
        public CliDispatcherOptions Options { get; set; }
        public object Context { get; set; }

        #endregion


        public CommandLineDispatchTask(string[] args = null)
        {
            this.Args = args;
        }

        public Task RunTask {
            get; private set;
        }

        public bool WaitForCompletion {
            get {
                return true;
            }
        }

        public void Start(CancellationToken? cancellationToken = null)
        {
            this.RunTask = Task.Factory.StartNew(() => CliDispatcher.Dispatch(Args, DispatcherObject, DispatcherType, Context, Options));
        }
    }


}
