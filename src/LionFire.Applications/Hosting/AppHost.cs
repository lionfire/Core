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
using LionFire.DependencyInjection;
using LionFire.MultiTyping;
using LionFire.Execution.Composition;
using LionFire.Assets;
using LionFire.Instantiating;
using System.Collections;
using LionFire.Composables;

namespace LionFire.Applications.Hosting
{

    public class AppHost : IAppHost, IReadonlyMultiTyped
    {
        MultiType multiType { get; set; } = new MultiType();

        public InjectionContext InjectionContext { get; private set; } = new InjectionContext();


        // REVIEW - Not sure this is needed or a good idea
        T IReadonlyMultiTyped.AsType<T>()
        {
            switch (typeof(T).Name)
            {
                case nameof(IServiceCollection):
                    return (T)ServiceCollection;
                case nameof(IServiceProvider):
                    return (T)ServiceProvider;
                default:
                    var result = multiType.AsType<T>();
                    if (result != null) return result;

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

        #region Dependency Injection

        public IServiceCollection ServiceCollection { get; private set; }

        #region ServiceProvider

        public IServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
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
                this.InjectionContext.ServiceProvider = value;
            }
        }
        private IServiceProvider serviceProvider;

        #endregion

        #endregion

        public IDictionary<string, object> Properties { get; private set; } = new Dictionary<string, object>();

        #region State

        public bool IsInitializeFrozen { get; private set; } = false;

        #endregion

        #region Register

        public IEnumerable<object> Children { get { return children; } }
        private List<object> children = new List<object>();

        //public IEnumerator<object> GetEnumerator() => Children.GetEnumerator(); // REVIEW - expose IAppHost.Components instead?

        public IAppHost Add<T>(T component)
            where T : class
        {
            //// REVIEW - only do this block if not added?
            //if (component is IConfigures<IServiceCollection> csc)
            //{
            //    csc.Configure(this.ServiceCollection);
            //}
            
            if (component is IAdding adding)
            {
                if (adding.OnAdding(this))
                {
                    children.Add(component);
                    multiType.SetType<T>(component);
                }
                else
                {
                }
            }
            else
            {
                children.Add(component);
            }
            return this;
        }

        #endregion

        #region Construction and Initialization

        public AppHost()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddSingleton(typeof(IAppHost), this);

            if (ManualSingleton<IAppHost>.Instance == null)
            {
                ManualSingleton<IAppHost>.Instance = this; // IsRootApplication == true
                InjectionContext.Default = this.InjectionContext;
            }
        }

        /// <summary>
        /// Default implementation is Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions.BuildServiceProvider().
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        protected virtual IServiceProvider BuildServiceProvider(IServiceCollection serviceCollection)
        {
            return serviceCollection.BuildServiceProvider(); // Microsoft extension method
        }

        public IAppHost InstantiateTemplates() // MOVE to ITemplateExtensions
        {
            foreach (var tComponent in children.OfType<ITemplate>().ToArray())
            {
                var component = tComponent.Create();
                this.Add(component);
                children.Remove(tComponent);
            }

            return this;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="mode"></param>
        ///// <returns></returns>
        //public IAppHost Bootstrap(BootstrapMode mode = BootstrapMode.Rebuild)
        //{

        //}

        public virtual void ResolveComponentDependencyProperties() => Children.TryResolveDependencies(ServiceProvider);


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
        /// Injects InjectionContext to components implementing IRequiresServices
        /// </summary>
        protected void InjectInjectionContextToComponents()
        {
            foreach (var component in children.OfType<IRequiresInjection>())
            {
                component.InjectionContext = this.InjectionContext;
            }
        }

        public IAppHost Initialize(BootstrapMode mode = BootstrapMode.Rebuild)
        {
            if (IsInitializeFrozen) throw new Exception("AppHost can no longer be initialized");

            if (IsRootApplication)
            {
                InjectionContext.Current = this.InjectionContext;
            }

            // FUTURE TODO: Use Resolution context?
            children.ResolveHandlesAsync().GetResultSafe();

            InstantiateTemplates();

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

            InjectInjectionContextToComponents();
            InjectServiceProviderToComponents();

            ResolveComponentDependencyProperties();


            children.OfType<TInitializable>().InitializeAll().Wait(); // Deprecated
            children.OfType<IInitializable2>().InitializeAll().Wait();

            if (mode == BootstrapMode.Discard)
            {
                ServiceCollection.Clear();
            }

            return this;
        }

        #endregion

        #region Derived Properties

        public bool IsRootApplication
        {
            get { return ManualSingleton<IAppHost>.Instance == this; }
        }

        #endregion

        #region Run

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        private List<Task> WaitForTasks { get; set; } = new List<Task>();
        private List<Task> Tasks { get; set; } = new List<Task>();

        public async Task Run()
        {
            Initialize();
            IsInitializeFrozen = true;

            WaitForTasks.Clear();
            Tasks.Clear();

            await children.OfType<IInitializable>().InitializeAll();
            //var validationErrors = await components.OfType<IInitializable2>().InitializeAll();

            #region Start

            List<Task> startTasks = new List<Task>();

            foreach (var component in children.OfType<IStartable>())
            {
                // Parallel start
                startTasks.Add(component.Start(tokenSource.Token));
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
