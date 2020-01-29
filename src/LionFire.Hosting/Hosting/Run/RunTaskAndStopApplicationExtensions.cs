using System;
using System.Threading.Tasks;
//using LionFire.Applications.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Dependencies;
using System.Threading;
using System.Reflection;
using System.Linq;

namespace LionFire.Hosting
{

    public static class RunTaskAndStopApplicationExtensions
    {
        public static async Task RunAsync(this IHostBuilder hostBuilder, INotifyDisposing disposable)
        {
            var mre = new ManualResetEvent(false);

            disposable.Disposing += Disposable_Disposing;
            await hostBuilder.RunAsync(() =>
            {
                mre.WaitOne();
                disposable.Disposing -= Disposable_Disposing;
            });


            void Disposable_Disposing(object obj)
            {
                mre.Set();
            }
        }

        public static Task RunAsync(this IHostBuilder hostBuilder, Action action)
        {
            return hostBuilder.RunAsync(services =>
            {
                action();
                return Task.CompletedTask;
            });
        }
        public static Task RunAsync(this IHostBuilder hostBuilder, Func<Task> taskFactory)
        {
            return hostBuilder.RunAsync(async _ =>
            {
                await taskFactory();
            });
        }

        public static Task RunAsync(this IHostBuilder hostBuilder, Action<IServiceProvider> action)
        {
            // TODO: a pure synchronous version?
            return hostBuilder.RunAsync(services =>
            {
                action(services);
                return Task.CompletedTask;
            });
        }

        public static void DeinitializeDependencyContext()
        {
            DependencyContext.AsyncLocal = null;

            if (DependencyContext.Current != null)
            {
                DependencyContext.Current.ServiceProvider = null;
            }
        }

        public static IHost InitializeDependencyContext(this IHost host)
        {

            if (LionFireEnvironment.IsMultiApplicationEnvironment)
            {
                DependencyLocatorConfiguration.UseServiceProviderToActivateSingletons = false;
                DependencyLocatorConfiguration.UseSingletons = false;

                if (DependencyContext.AsyncLocal != null) throw new AlreadyException("UNEXPECTED: LionFireEnvironment.IsMultiApplicationEnvironment == true && DependencyContext.AsyncLocal != null "); // FUTURE - deinit on Run complete?  Unit tests running in series?

                DependencyContext.AsyncLocal = new DependencyContext();
                DependencyContext.Current.ServiceProvider = host.Services;

                return host; // Don't set static Current
            }
            else
            {
                if (DependencyContext.Current == null)
                {
                    DependencyContext.Current = new DependencyContext();
                }

                if (DependencyContext.Current.ServiceProvider == null)
                {
                    DependencyContext.Current.ServiceProvider = host.Services;
                }
            }
            return host;
        }


        public static async Task RunAsync(this IHostBuilder hostBuilder, Func<IServiceProvider, Task> taskFactory)
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
                                 throw;
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
                DeinitializeDependencyContext();

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
