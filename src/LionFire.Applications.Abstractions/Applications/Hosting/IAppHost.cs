using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Applications.Hosting;
using LionFire.Execution;
//using LionFire.MultiTyping;
using LionFire.Execution.Composition;
using LionFire.Structures;
using LionFire.Composables;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace LionFire.Applications.Hosting
{
    // RENAME to ExecutionContainer?
    // - DependencyContext
    // - MultiTyped
    // - IComposable (children & multitype)
    // Can
    //  - Init/Run/Query result/Cleanup
    //[MultiTypeFromProperties]
    public interface IAppHost : 
        //IInitializable,
        IComposable<IAppHost>

    {
        #region Dependency Injection

        IServiceCollection ServiceCollection { get; }

        IServiceProvider ServiceProvider { get; }

        #endregion

        #region Configuration

        string AppId { get; set; }
        IDictionary<string, object> Properties { get; }

        #endregion

        bool IsRootApplication { get; }

        //IAppHost AddAsset<T>(string assetSubPath) where T : class; // RENAME AddAsset?

        #region Execution

        ///// <summary>
        ///// Build the ServiceProvider from the ServiceCollection and inject into any components already added.  This initializes the services registered so far that are required for futher app composition.
        ///// </summary>
        ///// <param name="mode"></param>
        //IAppHost Bootstrap(BootstrapMode mode = BootstrapMode.Rebuild);

        ///// <summary>
        ///// Optionally call this to prepare the application to run without running it.  If it is not invoked by the user, it will be invoked from the Run() method.  Invokations of this after initialization has completed will be ignored.  
        ///// </summary>
        //new Task<bool> Initialize();
        IAppHost Initialize(BootstrapMode mode = BootstrapMode.Rebuild);
        
        IAppHost Replace<T>(T toDelete, T toAdd) where T : class;

        /// <summary>
        /// Start application and return a task that waits for all ApplicationTasks with WaitForComplete = true to complete.
        /// (Also see Run extension method which will block until the application completes.)  (RENAME to Start, and use IStartable)
        /// </summary>
        /// <returns></returns>
        Task Run(CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region Shutdown

        //IObservable<bool> IsShuttingDown { get; }

        /// <summary>
        /// Sets IsShuttingDown to true.  All components that run perpetually until shutdown should monitor this flag and shut down in a timely manner.
        /// </summary>
        Task Shutdown(long millisecondsTimeout = 0, CancellationToken cancellationToken = default(CancellationToken));
        void SetManualSingletonFromService(Type serviceType);

        #endregion

        IAppHost Args(string[] args);
        IHostBuilder GenericHost(bool defaultHostBuilder = true);
        
    }
}

