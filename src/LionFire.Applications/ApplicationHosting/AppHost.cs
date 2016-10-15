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

        private List<object> components = new List<object>();

        public T Add<T>(T appComponent)
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

            if (ManualSingleton<IAppHost>.Instance == null)
            {
                ManualSingleton<IAppHost>.Instance = this;
            }
        }

        public bool IsRootApplication {
            get { return ManualSingleton<IAppHost>.Instance == this; }
        }


        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            foreach (var configurer in components.OfType<IAppConfigurer>())
            {
                configurer.Config(this);
            }
            return ServiceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Build ServiceProvider
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
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

            var unresolvedDependencies = await needsInitialization.TryResolveSet();
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

            foreach (var component in components.OfType<IAppTask>())
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
            if (Tasks.Count > 0)
            {
                var shutdownTask = Task.Factory.ContinueWhenAll(Tasks.ToArray(), _ => { });
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
