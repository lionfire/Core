using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Threading;
using LionFire.Structures;
using LionFire.Execution;
using TInitializable = LionFire.Execution.IInitializable;
using LionFire.Dependencies;
using LionFire.MultiTyping;
using LionFire.Execution.Composition;
using LionFire.Assets;
using LionFire.Instantiating;
using LionFire.DependencyInjection;

namespace LionFire.Applications.Hosting
{

    public class AppHost : IAppHost, IReadonlyMultiTyped
    {

        T IReadonlyMultiTyped.AsType<T>()
        {
            switch (typeof(T).Name)
            {
                case nameof(IServiceCollection):
                    return (T)ServiceCollection;
                case nameof(IServiceProvider):
                    return (T)ServiceProvider;
                default:
                    return null;
            }
        }

        #region Dependency Injection

        public IServiceCollection ServiceCollection { get; private set; }

        //public IServiceProvider ServiceProvider { get; private set; }

        #region ServiceProvider

        public IServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
            set
            {
                serviceProvider = value;
                if (IsRootApplication)
                {
                    InjectionContext.SetSingletonDefault(serviceProvider);
                }
            }
        }
        private IServiceProvider serviceProvider;

        #endregion

        #endregion

        public IDictionary<string, object> Properties { get; private set; } = new Dictionary<string, object>();

        #region State

        public bool IsInitialized { get; private set; } = false;

        #endregion

        #region Register

        public IEnumerable<object> Components { get { return components; } }
        private List<object> components = new List<object>();

        public IAppHost Add<T>(T appComponent)
        {
            components.Add(appComponent);
            return this;
        }
        public IAppHost Add<T>(string assetSubPath)
            where T : class
        {
            components.Add(new AssetReadHandle<T>(assetSubPath));
            return this;
        }

        IAppHost IComposableExecutable<IAppHost>.Add(IConfigures component)
        {
            components.Add(component);
            return this;
        }
        IAppHost IComposableExecutable<IAppHost>.Add(IInitializes component)
        {
            components.Add(component);
            return this;
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

            if (ManualSingleton<IAppHost>.Instance == null)
            {
                ManualSingleton<IAppHost>.Instance = this;
            }
        }

        public bool IsRootApplication
        {
            get { return ManualSingleton<IAppHost>.Instance == this; }
        }


        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var configContext = this;
            foreach (var configurer in components.OfType<IConfigures<IServiceCollection>>())
            {
                configurer.Configure(ServiceCollection);
            }
            foreach (var configurer in components.OfType<IConfigures<IAppHost>>())
            {
                configurer.Configure(this);
            }
            //foreach (var configurer in components.OfType<IAppConfigurer>())
            //{
            //    configurer.Config(this);
            //}


            return ServiceCollection.BuildServiceProvider();
        }

        public IAppHost LoadHandles()
        {
            Parallel.ForEach(components.OfType<IReadHandle<object>>(), async rh => await rh.TryLoadNonNull());


            foreach (var component in components.OfType<IReadHandle<object>>().ToArray())
            {
                this.Add(component.Object);
                components.Remove(component);
            }

            return this;
        }

        public IAppHost InstantiateTemplates()
        {
            foreach (var tComponent in components.OfType<ITemplate>().ToArray())
            {
                var component = tComponent.Create();
                this.Add(component);
                components.Remove(tComponent);
            }

            return this;
        }

        /// <summary>
        /// Build ServiceProvider
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public IAppHost Bootstrap(BootstrapMode mode = BootstrapMode.Rebuild)
        {
            LoadHandles();
            InstantiateTemplates();

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

            if (IsRootApplication)
            {
                ManualSingleton<IServiceProvider>.Instance = ServiceProvider;
            }
        }

        public async Task<bool> Initialize()
        {
            if (IsInitialized) return IsInitialized;

            Bootstrap();

            var needsInitialization = new List<TInitializable>(components.OfType<TInitializable>());
            int componentsRequiringInit = needsInitialization.Count;
            List<TInitializable> stillNeedsInitialization = null;

            var unresolvedDependencies = await needsInitialization.TryResolveSet(this.ServiceProvider);
            if (unresolvedDependencies != null && unresolvedDependencies.Count > 0)
            {
                throw new HasUnresolvedDependenciesException(unresolvedDependencies);
            }

            do
            {
                //Dictionary<object, UnsatisfiedDependencies> uds = new Dictionary<object, Dependencies.UnsatisfiedDependencies>();

                if (stillNeedsInitialization != null)
                {
                    needsInitialization = stillNeedsInitialization;
                }
                stillNeedsInitialization = new List<TInitializable>();

                foreach (var component in needsInitialization)
                {
                    //UnsatisfiedDependencies ud;
                    //if (!component.TryResolveDependencies(out ud, ServiceProvider))
                    //{
                    //    uds.Add(component, ud);
                    //    stillNeedsInitialization.Add(component);
                    //}
                    //else 
                    if (await component.Initialize() == false)
                    {
                        stillNeedsInitialization.Add(component);
                    }
                }
                if (stillNeedsInitialization.Count == componentsRequiringInit)
                {
                    var msg = $"No progress made on initializing {componentsRequiringInit} remaining components: " + stillNeedsInitialization.Select(c => c.ToString()).Aggregate((x, y) => x + ", " + y);
                    //if (uds.Count > 0)
                    //{
                    //    msg += " Missing dependencies: ";
                    //    foreach (var kvp in uds)
                    //    {
                    //        if (kvp.Value.Count == 0) continue;
                    //        msg += $"Object of type {kvp.Key.GetType().Name} needs: ";
                    //        bool isFirst = true;
                    //        foreach (var d in kvp.Value)
                    //        {
                    //            if (isFirst) isFirst = false; else msg += ", ";
                    //            msg += d.Description;
                    //        }
                    //    }
                    //}
                    throw new Exception(msg);
                }

            } while (stillNeedsInitialization.Count > 0);

            IsInitialized = true;
            return IsInitialized;
        }

        #endregion

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        private List<Task> WaitForTasks { get; set; } = new List<Task>();
        private List<Task> Tasks { get; set; } = new List<Task>();

        public async Task Run()
        {
            await Initialize();

            WaitForTasks.Clear();
            Tasks.Clear();

            List<Task> startTasks = new List<Task>();

            foreach (var component in components.OfType<IStartable>())
            {
                // Parallel start
                startTasks.Add(component.Start(tokenSource.Token));
            }

            Task.WaitAll(startTasks.ToArray());

            foreach (var component in components.OfType<IHasRunTask>())
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
    }

}
