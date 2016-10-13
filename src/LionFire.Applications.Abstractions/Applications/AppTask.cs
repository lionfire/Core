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

    public class AppTask : IAppTask, IAppInitializer, IRequiresServices
    {

        #region Configuration

        /// <summary>
        /// Returns true if successful, false if it should be tried again after initializing other application components
        /// </summary>
        public Func<bool> TryInitializeAction { get; set; } = () => true;
        public Action RunAction { get; set; }

        public bool RunSynchronously { get; set; } = false;
        public bool WaitForCompletion { get; set; } = true;

        public ExecutionFlags ExecutionFlags { get; set; }

        #endregion

        #region Relationships

        IServiceProvider IRequiresServices.ServiceProvider { set { this.ServiceProvider = value; } }
        protected IServiceProvider ServiceProvider { get; private set; }

        #endregion

        #region Construction

        public AppTask() { }
        public AppTask(Action run = null, Func<bool> tryInitialize = null)
        {
            this.RunAction = run;
            if (tryInitialize != null)
            {

                this.TryInitializeAction = tryInitialize;
            }
        }

        #endregion

        #region Initialization

        public virtual bool TryInitialize()
        {
            return TryInitializeAction();
        }

        protected virtual void Run() { }

        public virtual void Start(System.Threading.CancellationToken? cancellationToken = null)
        {
            if (!TryInitialize()) { throw new Exception($"{this} failed to initialize.  Cannot start it."); }

            if (cancellationToken.HasValue) { this.CancellationToken = cancellationToken; }

            var runAction = RunAction ?? Run;

            if (RunSynchronously)
            {
                runAction();
            }
            else
            {
                this.RunTask = Task.Factory.StartNew(runAction);
            }
        }

        #endregion

        #region State

        protected CancellationToken? CancellationToken { get; set; }

        public Task RunTask { get; private set; }

        #endregion
    }

}
