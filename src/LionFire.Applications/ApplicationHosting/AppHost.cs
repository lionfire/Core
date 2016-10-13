using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Threading;
using LionFire.Structures;
using LionFire.Execution;

namespace LionFire.Applications.Hosting
{

    public class AppHost : IAppHost
    {

        #region Dependency Injection

        public IServiceCollection ServiceCollection { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        #endregion

        public IDictionary<string, object> Properties { get; private set; } = new Dictionary<string, object>();

        #region State

        public bool IsInitialized { get; private set; } = false;

        #endregion

        #region Register

        private List<IAppComponent> components = new List<IAppComponent>();

        public T Add<T>(T appComponent)
            where T : IAppComponent
        {
            components.Add(appComponent);
            return appComponent;
        }


        public T Add<T>()
            where T : IAppTask, new()
        {
            var appTask = new T();
            Add(appTask);
            return appTask;
        }

        #endregion

        #region Construction and Initialization

        public AppHost()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddSingleton(typeof(IAppTask), this);

            if(ManualSingleton<IAppHost>.Instance == null){
                ManualSingleton<IAppHost>.Instance = this;
            }
        }

        public bool IsRootApplication
{
get{return ManualSingleton<IAppHost>.Instance == this;}
}


        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            foreach (var configurer in components.OfType<IAppConfigurer>())
            {
                configurer.Config(this);
            }
            return ServiceCollection.BuildServiceProvider();
        }

        public IAppHost Bootstrap(BootstrapMode mode = BootstrapMode.Rebuild)
        {
            // FUTURE: Consider reusing existing service object instances
            BuildServiceProvider();

            if (mode == BootstrapMode.Discard)
            {
                ServiceCollection.Clear();
            }
            return this;
        }

        protected void BuildServiceProvider()
        {
            ServiceProvider = ConfigureServices(this.ServiceCollection);

            foreach (var component in components.OfType<IRequiresServices>())
            {
                component.ServiceProvider = ServiceProvider;
            }

            if(IsRootApplication){
            ManualSingleton<IServiceProvider>.Instance = ServiceProvider;
            }
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            Bootstrap();

            var needsInitialization = new List<IAppInitializer>(components.OfType<IAppInitializer>());
            int componentsRequiringInit = needsInitialization.Count;
            List<IAppInitializer> stillNeedsInitialization = null;
            do
            {
                if (stillNeedsInitialization != null)
                {
                    needsInitialization = stillNeedsInitialization;
                }
                stillNeedsInitialization = new List<IAppInitializer>();

                foreach (var component in needsInitialization)
                {
                    if (!component.TryInitialize())
                    {
                        stillNeedsInitialization.Add(component);
                    }
                }
                if (stillNeedsInitialization.Count == componentsRequiringInit)
                {
                    throw new Exception($"No progress made on initializing {componentsRequiringInit} remaining components: " + stillNeedsInitialization.Select(c => c.ToString()).Aggregate((x, y) => x + ", " + y));
                }

            } while (stillNeedsInitialization.Count > 0);

            IsInitialized = true;
        }

        #endregion

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        private List<Task> WaitForTasks { get; set; } = new List<Task>();
        private List<Task> Tasks { get; set; } = new List<Task>();

        public async Task Run(Func<Task> runMethod = null)
        {
            Initialize();

            WaitForTasks.Clear();
            Tasks.Clear();

            foreach (var component in components.OfType<IAppTask>())
            {
                component.Start(tokenSource.Token);

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

        #region Shutdown

        //public int ShutdownMillisecondsDelay = 60000;  // FUTURE?

        public Task Shutdown(long millisecondsTimeout = 0)
        {
            tokenSource.Cancel();
            var shutdownTask = Task.Factory.ContinueWhenAll(Tasks.ToArray(), _ => { Tasks.Clear(); });
            return shutdownTask;
        }

        public Task WaitForShutdown()
        {
            var shutdownTask = Task.Factory.ContinueWhenAll(Tasks.ToArray(), _ => { });
            return shutdownTask;
        }

        #endregion
    }

}
