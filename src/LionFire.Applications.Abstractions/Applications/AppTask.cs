using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Applications.Hosting;
using LionFire.Structures;
using LionFire.Execution;

namespace LionFire.Applications
{

    public class AppTask : IAppTask, IInitializable, IRequiresServices
    {

        #region Configuration

        /// <summary>
        /// Returns true if successful, false if it should be tried again after initializing other application components
        /// </summary>
        public Func<bool> TryInitializeAction { get; set; } = () => true;
        public Func<Task> RunTaskFunction { get; set; }

        public bool RunSynchronously { get; set; } = false;
        public bool WaitForCompletion { get; set; } = true;

        public ExecutionFlag ExecutionFlags { get; set; } = ExecutionFlag.WaitForRunCompletion;

        //public static ExecutionFlag DefaultAppTaskExecutionFlags = ExecutionFlag.WaitForRunCompletion;
        #endregion

        #region Relationships

        IServiceProvider IRequiresServices.ServiceProvider { get { return ServiceProvider; } set { this.ServiceProvider = value; } }
        protected IServiceProvider ServiceProvider { get; private set; }

        #endregion

        #region Construction

        public AppTask() { }
        public AppTask(Action run = null, Func<bool> tryInitialize = null) : this(() => Task.Run(run), tryInitialize)
        {
            //this.RunTaskFunction = () => Task.Run(run);
            //if (tryInitialize != null)
            //{
            //    this.TryInitializeAction = tryInitialize;
            //}
        }
        public AppTask(Func<Task> run, Func<bool> tryInitialize = null)
        {
            this.RunTaskFunction = run;
            if (tryInitialize != null)
            {
                this.TryInitializeAction = tryInitialize;
            }
        }

        #endregion

        #region Initialization

        public virtual Task<bool> Initialize(/*CancellationToken cancellationToken = default*/)
        {
            return Task.FromResult(TryInitializeAction());
        }

        protected virtual Task Run() { return Task.CompletedTask; }

        //public virtual async Task StartOld(/*System.Threading.CancellationToken cancellationToken = default*/)
        //{
        //    if (await Initialize().ConfigureAwait(false) == false) { throw new Exception($"{this} failed to initialize.  Cannot start it."); }

        //    //if (cancellationToken.HasValue) { this.CancellationToken = cancellationToken; }

        //    var runAction = RunAction ?? new Action(() => { Run().Wait();  });

        //    if (RunSynchronously)
        //    {
        //        runAction();
        //    }
        //    else
        //    {
        //        this.RunTask = Task.Run(RunAction);
        //    }
        //}
        public virtual async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (await Initialize(/*cancellationToken*/).ConfigureAwait(false) == false) { throw new Exception($"{this} failed to initialize.  Cannot start it."); }

            //if (cancellationToken.HasValue) { this.CancellationToken = cancellationToken; }

            if (RunTaskFunction != null)
            {
                await RunTaskFunction();
            }
            else
            {
                await Run();
            }
        }

        #endregion

        #region State

        protected CancellationToken? CancellationToken { get; set; }

        public Task RunTask { get; private set; }

        #endregion
    }

}
