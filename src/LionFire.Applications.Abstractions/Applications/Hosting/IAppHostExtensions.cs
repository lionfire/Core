using LionFire.Execution.Composition;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Execution;

namespace LionFire.Applications.Hosting
{
    public static class IAppHostExtensions
    {

        public static IAppHost ConfigureServices(this IAppHost appHost, Action<IServiceCollection> action)
        {
            action(appHost.ServiceCollection);
            return appHost;
        }

        public static IAppHost ConfigureServices(this IAppHost app, IConfigures<IServiceCollection> configures)
        {
            configures.Configure(app.ServiceCollection);
            return app;
        }

        #region DependencyInjection pass-through

        public static IAppHost AddSingleton<T>(this IAppHost host)
            where T : class
        {
            host.ServiceCollection.AddSingleton<T>();
            return host;
        }
        public static IAppHost AddSingleton<T>(this IAppHost host, T implementationInstance)
            where T : class
        {
            host.ServiceCollection.AddSingleton<T>(implementationInstance);
            return host;
        }

        public static IAppHost AddSingleton<TService, TImplementation>(this IAppHost host)
            where TService : class
            where TImplementation : class, TService
        {
            host.ServiceCollection.AddSingleton<TService, TImplementation>();
            return host;
        }

        public static IAppHost AddSingleton<TService, TImplementation>(this IAppHost host, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            host.ServiceCollection.AddSingleton<TService, TImplementation>(implementationFactory);
            return host;
        }

        public static IAppHost AddSingleton(this IAppHost host, Type serviceType, Type implementationType)
        {
            host.ServiceCollection.AddSingleton(serviceType, implementationType);
            return host;
        }

        public static IAppHost AddSingleton(this IAppHost host, Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            host.ServiceCollection.AddSingleton(serviceType, implementationFactory);
            return host;
        }

        // TODO: others for singleton, transient, etc. ??

        #endregion

        public static IAppHost Add<T>(this IAppHost host)
            where T : class, new()
        {
            host.Add(new T());
            return host;
        }

        /// <summary>
        /// Adds a task to the application
        /// </summary>
        public static IAppHost Add(this IAppHost host, Action action, Func<bool> tryInitialize = null)
        {
            host.Add(new AppTask(action, tryInitialize));
            return host;
        }

        public static IAppHost AddInit(this IAppHost host, Func<IAppHost, bool> tryInitialize)
        {
            host.Add(new AppInitializer(tryInitialize));
            return host;
        }

        public static IAppHost AddInit(this IAppHost host, Action<IAppHost> initialize)
        {
            host.Add(new AppInitializer(initialize));
            return host;
        }

        /// <summary>
        /// Start application and wait until all ApplicationTasks with WaitForComplete = true to complete.
        /// </summary>
        public static async Task<IAppHost> RunNowAndWait(this IAppHost host, Action runMethod)
        {
            if (runMethod != null) host.Add(new AppTask(runMethod));
            await host.Run();
            return host;
        }

        /// <summary>
        /// Start application and wait until all ApplicationTasks with WaitForComplete = true to complete.
        /// </summary>
        public static async Task<IAppHost> RunNowAndWait(this IAppHost host, Func<Task> runMethod = null)
        {
            if (runMethod != null) host.Add(new AppTask(runMethod));
            await host.Run();
            return host;
        }

        public static IAppHost Run(this IAppHost host, Func<Task> runMethod)
        {
            if (runMethod != null) host.Add(new AppTask(runMethod));
            host.Run();
            return host;
        }
        public static IAppHost Run(this IAppHost host, Action runMethod)
        {
            if (runMethod != null) host.Add(new AppTask(runMethod));
            host.Run();
            return host;
        }


        /// <summary>
        /// Initialize the services registered so far that are required for futher app composition
        /// </summary>
        //public static IAppHost Bootstrap(this IAppHost host)
        //{
        //    host.Bootstrap();
        //    return host;
        //}



    }
}
