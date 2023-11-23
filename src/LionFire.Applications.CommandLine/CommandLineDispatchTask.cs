using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.CommandLine;
using System.Threading;
using LionFire.CommandLine.Dispatching;
using LionFire.Execution;

namespace LionFire.Applications.Hosting;

public class CommandLineDispatchTask : IHasExecutionFlags, IStartable, IHasRunTask
{
    #region Parameters

    public string[] Args { get; set; }
    public object DispatcherObject { get; set; }
    public Type DispatcherType { get; set; }
    public CliDispatcherOptions Options { get; set; }
    public object Context { get; set; }

    #endregion

    #region Construction

    #endregion

    public CommandLineDispatchTask(string[] args = null)
    {
        this.Args = args;
    }
    
    #region Execution

    #region Configuration

    public ExecutionFlag ExecutionFlags {
        get {
            return ExecutionFlag.WaitForRunCompletion;
        }
    }

    #endregion

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        //CancellationToken cancellationToken = default;
        this.RunTask = Task.Factory.StartNew(() => CliDispatcher.Dispatch(Args, DispatcherObject, DispatcherType, Context, Options));
        return Task.CompletedTask;
    }

    public Task RunTask {
        get; private set;
    }

    #endregion

}
