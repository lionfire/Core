using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Threading;
using LionFire.Structures;

namespace LionFire.Applications.Hosting
{

    public class ApplicationHost : IAppHost
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

        public ApplicationHost()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddSingleton(typeof(IAppTask), this);

            ManualSingleton<IAppHost>.Instance = this;
        }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            foreach (var configurer in components.OfType<IAppConfigurer>())
            {
                configurer.Config(this);
            }
            return ServiceCollection.BuildServiceProvider();
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            ServiceProvider = ConfigureServices(this.ServiceCollection);
            ManualSingleton<IServiceProvider>.Instance = ServiceProvider;

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

        public async Task Start()
        {
            Initialize();

            foreach (var component in components.OfType<IAppTask>())
            {
                component.Start(tokenSource.Token);
                if (component.WaitForCompletion && component.Task != null)
                {
                    WaitForTasks.Add(component.Task);
                }
            }
            await WaitForShutdown();
        }

        #region Shutdown

        public Task Shutdown(long millisecondsTimeout = 0)
        {
            tokenSource.Cancel();
            var shutdownTask = Task.Factory.ContinueWhenAll(WaitForTasks.ToArray(), _ => { WaitForTasks.Clear(); });
            return shutdownTask;
        }

        public Task WaitForShutdown()
        {
            var shutdownTask = Task.Factory.ContinueWhenAll(WaitForTasks.ToArray(), _ => {; });
            return shutdownTask;
        }

        #endregion
    }

}
