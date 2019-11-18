using System;
using System.Threading.Tasks;
using LionFire.Applications.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Dependencies;

namespace LionFire.Hosting
{
    public static class RunTaskAndStopApplicationExtensions
    {
        public static Task Run(this IHostBuilder hostBuilder, Action action)
        {
            return hostBuilder.Run(services =>
            {
                action();
                return Task.CompletedTask;
            });
        }
        public static Task Run(this IHostBuilder hostBuilder, Func<Task> taskFactory)
        {
            return hostBuilder.Run(async _ =>
            {
                await taskFactory();
            });
        }

        public static Task Run(this IHostBuilder hostBuilder, Action<IServiceProvider> action)
        {
            return hostBuilder.Run(services =>
            {
                action(services);
                return Task.CompletedTask;
            });
        }

        public static IHost InitializeDependencyContext(this IHost host)
        {
            if (DependencyContext.Current == null)
            {
                DependencyContext.Current = new DependencyContext();
            }

            if (DependencyContext.Current.ServiceProvider == null)
            {
                DependencyContext.Current.ServiceProvider = host.Services;
            }
            return host;
        }

        public static async Task Run(this IHostBuilder hostBuilder, Func<IServiceProvider, Task> taskFactory)
        {
            var tcs = new TaskCompletionSource<object>();

            Exception exception = null;

            await hostBuilder.ConfigureServices((context, sc) =>
                 {
                     sc
                         .AddSingleton(new RunOptions(async services =>
                         {
                             //Debug.WriteLine("Run starting");
                             try
                             {
                                 await taskFactory(services).ConfigureAwait(false);
                             }
                             catch (Exception ex)
                             {
                                 exception = ex;
                             }
                             //Debug.WriteLine("Run done");
                         }))
                         .AddHostedService<RunTaskAndStopApplication>()
                        ;
                 })
            .Build()
            .InitializeDependencyContext()
            .RunAsync()
            .ContinueWith(t =>
            {
                if (DependencyContext.Current != null)
                {
                    DependencyContext.Current.ServiceProvider = null;
                }
                if (exception != null)
                {
                    tcs.SetException(exception);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            await tcs.Task;
            //return tcs.Task;
        }
    }
}
