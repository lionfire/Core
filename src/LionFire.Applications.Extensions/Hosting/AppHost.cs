using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Composables;
using LionFire.Dependencies;
using LionFire.Execution;
using LionFire.MultiTyping;
using LionFire.Referencing;
using LionFire.Structures;
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TInitializable = LionFire.Execution.IInitializable;

namespace LionFire.Applications.Hosting
{
    // Derive from new ExecutionContainer, move most stuff there?
    public class AppHost : ExecutablesHost<AppHost>, IAppHost, IReadOnlyMultiTyped, Microsoft.Extensions.Hosting.IHost
    {
        private static IAppHost _CreateUnitTestApp()
        {
            if (CreateUnitTestApp == null)
            {
                return null;
            }

            IsCreatingUnitTestApp = true;
            return CreateUnitTestApp();
        }
        public static Func<IAppHost> CreateUnitTestApp = () => new AppHost().Initialize();
        private volatile static bool IsCreatingUnitTestApp = false;

        public static IAppHost MainApp { get => ManualSingleton<IAppHost>.Instance; protected set => ManualSingleton<IAppHost>.Instance = value; }

        #region MultiType

        protected readonly MultiType multiType = new MultiType();

        // REVIEW - Not sure this is needed or a good idea
        T SReadOnlyMultiTyped.AsType<T>()
        {
            switch (typeof(T).Name)
            {
                case nameof(IServiceCollection):
                    return (T)ServiceCollection;
                case nameof(IServiceProvider):
                    return (T)ServiceProvider;
                default:
                    var result = multiType.AsType<T>();
                    if (result != null)
                    {
                        return result;
                    }

                    //if (ServiceProvider == null)
                    //{
                    //    return null;
                    //}
                    //else
                    //{
                    //    return ServiceProvider.GetService<T>();
                    //}
                    return null;
            }
        }

        public IEnumerable<T> OfType<T>() where T : class => multiType.OfType<T>();
        public object AsType(Type T) => multiType.AsType(T);
        public IEnumerable<object> OfType(Type T) => multiType.OfType(T);

        #endregion

        IAppHost IComposable<IAppHost>.Add<TComponent>(TComponent component) { base.Add(component); return this; }

        #region Injection

        public DependencyContext DependencyContext { get; private set; } = new DependencyContext();

        #endregion

        #region Microsoft.Extensions.DependencyInjection

        public IServiceCollection ServiceCollection { get; private set; }

        #region ServiceProvider

        public IServiceProvider ServiceProvider
        {
            get => serviceProvider;
            set
            {
                //if (IsRootApplication)
                //{
                //    if (ManualSingleton<IServiceProvider>.Instance != null && ManualSingleton<IServiceProvider>.Instance != serviceProvider)
                //    {
                //        throw new AlreadyException("ManualSingleton<IServiceProvider>.Instance already set to an unknown ServiceProvider, but this is the root AppHost.");
                //    }
                //}
                serviceProvider = value;

                if (SetManualSingletons != null)
                {
                    foreach (var type in SetManualSingletons)
                    {
                        ManualSingleton.SetInstance(serviceProvider.GetService(type), type);
                    }
                    SetManualSingletons = null;
                }

                DependencyContext.ServiceProvider = value;
            }
        }
        private IServiceProvider serviceProvider;

        public void SetManualSingletonFromService(Type serviceType)
        {
            if (!object.ReferenceEquals(MainApp, this))
            {
                return;
            }

            if (SetManualSingletons == null)
            {
                SetManualSingletons = new List<Type>();
            }

            SetManualSingletons.Add(serviceType);
        }
        private List<Type> SetManualSingletons; // MOVE to AppHostBuilder

        #endregion

        #endregion


        // TODO: Put this into the Multitype?  Maybe make it a common extensionmethod thing?
        public IDictionary<string, object> Properties { get; private set; } = new Dictionary<string, object>();

        /// <summary>
        /// Can be used to conduct multiple unit tests in one process.
        /// 
        /// Resets these to null:
        ///  - MainApp
        ///  - DependencyContext.Default 
        ///  - DependencyContext.Current
        ///  - ManualSingleton.Instance for all types
        /// </summary>
        public static void Reset()
        {
            MainApp = null;
            DependencyContext.Default = null;
            DependencyContext.Current = null;
            ManualSingletonRegistrar.ResetAll();
        }

        #region Construction and Initialization

        public static bool AutoMultipleAppsWithoutSpecifyingPrimaryAppFalse = true;

        private static object ctorLock = new object();

        public static bool DetectUnitTestMode = true;

        public string AppId { get; set; }

        public AppHost(string appId = null, bool primaryApp = true)
        {
            this.AppId = appId;

            lock (ctorLock)
            {
                ServiceCollection = new ServiceCollection();
                ServiceCollection.AddSingleton(typeof(IAppHost), this);

                if (MainApp == null)
                {
                    if (DetectUnitTestMode && !IsCreatingUnitTestApp && UnitTestingDetection.IsInUnitTest && CreateUnitTestApp != null)
                    {
                        _CreateUnitTestApp();
                        if (MainApp == null)
                        {
                            throw new Exception("CreateUnitTestApp did not set MainApp");
                        }
                        primaryApp = true;
                    }
                }

                if (MainApp == null)
                {
                    MainApp = this; // IsRootApplication == true
                }
                else
                {
                    if (AutoMultipleAppsWithoutSpecifyingPrimaryAppFalse)
                    {
                        primaryApp = false;
                    }

                    if (primaryApp)
                    {
                        throw new Exception("Already has a AppHost.MainApp set.  Create AppHost with primaryApp set to false to create multiple applications, or else set AppHost.MainApp to null first before creating another AppHost.");
                    }
                }

                throw new NotImplementedException("Old - using DependencyContext.Initialize... instead.");
                //if (primaryApp)
                //{
                //    //if (DependencyContext.Default != null)
                //    //{
                //    //    throw new Exception($"Already has an DependencyContext.Default.  Create AppHost with {nameof(primaryApp)} set to false to create multiple applications, or else set DependencyContext.Default to null first before creating another AppHost.");
                //    //}
                //    DependencyContext.Default = DependencyContext;
                //}
                //else
                //{
                //    DependencyContext.AsyncLocal = DependencyContext;
                //}
            }
        }
               
        /// <summary>
        /// Default implementation is Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions.BuildServiceProvider().
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        protected virtual IServiceProvider BuildServiceProvider(IServiceCollection serviceCollection)
            => serviceCollection.BuildServiceProvider(); // Microsoft extension method
        

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="mode"></param>
        ///// <returns></returns>
        //public IAppHost Bootstrap(BootstrapMode mode = BootstrapMode.Rebuild)
        //{
        //}

        public virtual void ResolveComponentDependencyProperties() => children.TryResolveDependencies(ServiceProvider);

        /// <summary>
        /// Injects ServiceProvider to components implementing IRequiresServices
        /// </summary>
        protected void InjectServiceProviderToComponents()
        {
            foreach (var component in children.OfType<IRequiresServices>())
            {
                component.ServiceProvider = ServiceProvider;
            }
        }

        /// <summary>
        /// Injects DependencyContext to components implementing IRequiresServices
        /// </summary>
        protected void InjectDependencyContextToComponents()
        {
            foreach (var component in children.OfType<IRequiresInjection>())
            {
                component.DependencyContext = DependencyContext;
            }
        }

        public IAppHost Initialize(BootstrapMode mode = BootstrapMode.Rebuild)
        {
            if (IsInitializeFrozen)
            {
                throw new Exception("AppHost can no longer be initialized");
            }

            if (IsRootApplication)
            {
                DependencyContext.Current = DependencyContext;
            }

            // FUTURE TODO: Use Resolution context?
            children.ResolveHandlesAsync().GetResultSafe();

            // InstantiateTemplates(); MOVED to ITemplateExtensions.InstantiateTemplates.  Put it in a IConfigures if still desired

            foreach (var configurer in children.OfType<IConfigures<IAppHost>>())
            {
                configurer.Configure(this);
            }

            foreach (var configurer in children.OfType<IConfigures<IServiceCollection>>())
            {
                configurer.Configure(ServiceCollection);
            }

            // FUTURE option: Consider reusing existing service object instances during a rebuild?

            ServiceProvider = BuildServiceProvider(ServiceCollection);

            InjectDependencyContextToComponents();
            InjectServiceProviderToComponents();

            ResolveComponentDependencyProperties();

            // LIMITATION: These can't be lazily initialized as a batch.
            children.OfType<TInitializable>().InitializeAll().Wait(); // Deprecated
            children.OfType<IInitializable2>().InitializeAll().Wait();
            children.OfType<IInitializable3>().InitializeAll().Wait();
            children.OfType<IInitializerFor<IAppHost>>().InitializeAll(this).Wait();

            if (mode == BootstrapMode.Discard)
            {
                ServiceCollection.Clear();
            }

            return this;
        }

        #endregion

        #region State

        public bool IsInitializeFrozen { get; private set; } = false;

        #endregion
        #region Derived Properties

        public bool IsRootApplication => object.ReferenceEquals(this, MainApp);

        #endregion


        #region Run

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private List<Task> WaitForTasks { get; set; } = new List<Task>();
        private List<Task> Tasks { get; set; } = new List<Task>();

        public IServiceProvider Services => throw new NotImplementedException();

        public IEnumerable<object> SubTypes => throw new NotImplementedException();

        public object this[Type type] => throw new NotImplementedException();

        public class StateMachineWrapper<TState, TTransition>
        {
            static StateMachineWrapper()
            {

            }

        }

        public async Task Run(CancellationToken cancellationToken = default(CancellationToken))
        {
            Initialize();
            IsInitializeFrozen = true;

            WaitForTasks.Clear();
            Tasks.Clear();

            await children.OfType<IInitializable>().InitializeAll();
            //var validationErrors = await components.OfType<IInitializable2>().InitializeAll();

            // FUTURE: Handle 

            #region Start

            List<Task> startTasks = new List<Task>();

            foreach (var component in children.OfType<IStartable>())
            {
                // Parallel start
                startTasks.Add(component.StartAsync(tokenSource.Token));
            }

            Task.WaitAll(startTasks.ToArray());

            #endregion


            foreach (var component in children.OfType<IHasRunTask>())
            {
                if (component.WaitForRunCompletion() == true && component.RunTask != null)
                {
                    WaitForTasks.Add(component.RunTask);
                }
                if (component.RunTask != null)
                {
                    Tasks.Add(component.RunTask);
                }
            }

            await WaitForShutdown();
        }

        #endregion


        #region Shutdown

        //public int ShutdownMillisecondsDelay = 60000;  // FUTURE?

        public Task Shutdown(long millisecondsTimeout = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            tokenSource.Cancel();
            var shutdownTask = Task.Factory.ContinueWhenAll(Tasks.ToArray(), _ => { Tasks.Clear(); });
            return shutdownTask;
        }

        public Task WaitForShutdown()
        {
            var tasks = WaitForTasks;

            if (tasks.Count > 0)
            {
                var shutdownTask = Task.Factory.ContinueWhenAll(tasks.ToArray(), _ => { });
                return shutdownTask;
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        #endregion

        #region Microsoft.Extensions.IHost

        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken)) => Run(cancellationToken);
        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken)) => Shutdown(cancellationToken: cancellationToken);

        #endregion

        public IAppHost Args(string[] args = null)
        {
            this.args = args;
            return this;
        }
        private string[] args = null;


        /// <summary>
        /// Parameters are only used the first time this is invoked.
        /// </summary>
        /// <param name="defaultHostBuilder"></param>
        /// <returns></returns>
        public IHostBuilder GenericHost(bool defaultHostBuilder = true)
        {
            if (hostBuilder == null)
            {
                hostBuilder = CreateGenericHost(args, defaultHostBuilder);
            }
            return hostBuilder;
        }
        private IHostBuilder hostBuilder;
   
        private IHostBuilder CreateGenericHost(string[] args = null, bool defaultHostBuilder = true)
        {
            
            var hostBuilder = defaultHostBuilder ? Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args) : new HostBuilder();
            hostBuilder.ConfigureContainer<IServiceContainer>((context, services) =>
            {
                throw new NotImplementedException("TODO: How to merge AppHost and HostBuilder?  Get AppHost to ");
            });
            // TODO: hook in?

            return hostBuilder;
        }


        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected event Action Disposing;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disposing?.Invoke();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        // TODO: Move this to IExecutableHost somehow?
        public virtual IAppHost Replace<TComponent>(TComponent toRemove, TComponent toAdd)
            where TComponent : class
        {
            // TODO: Fire only one event
            children.Remove(toRemove);
            Add(toAdd);
            return this;
        }

    }
}
