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

        #region Function injection

        public static Task RunAsync<T1>(this IHostBuilder hostBuilder, Action<T1> action)
            => RunAsync<T1>(hostBuilder, (Func<T1, Task>)(async t1 => await Task.Run(() => action(t1))));

        public static Task RunAsync<T1>(this IHostBuilder hostBuilder, Func<T1, Task> taskFactory)
        {
            return hostBuilder.RunAsync(new Func<IServiceProvider, Task>(s =>
                {
                    return taskFactory(s.GetRequiredService<T1>());
                }));
        }

        public static void Run(this IHostBuilder hostBuilder, Func<IServiceProvider, Task> taskFactory)
        {
            hostBuilder.RunAsync(new Func<IServiceProvider, Task>(s =>
            {
                return taskFactory(s.GetRequiredService<IServiceProvider>());
            })).Wait();
        }

        public static void Run(this IHostBuilder hostBuilder, Func<Task> taskFactory)
        {
            hostBuilder.RunAsync(new Func<IServiceProvider, Task>(_ => // REFACTOR: unnecessary IServiceProvider
            {
                return taskFactory();
            })).Wait();
        }
        public static void Run(this IHostBuilder hostBuilder, Action action)
        {
            hostBuilder.RunAsync(new Func<IServiceProvider, Task>(_ => // REFACTOR: unnecessary IServiceProvider
            {
                return Task.Run(action);
            })).Wait();
        }


        public static void Run<T1>(this IHostBuilder hostBuilder, Func<T1, Task> taskFactory)
        {
            hostBuilder.RunAsync(new Func<IServiceProvider, Task>(s =>
            {
                return taskFactory(s.GetRequiredService<T1>());
            })).Wait();
        }

        public static Task RunAsync<T1, T2>(this IHostBuilder hostBuilder, Func<T1, T2, Task> taskFactory)
        {
            return hostBuilder.RunAsync(new Func<IServiceProvider, Task>(async s =>
            {
                var p1 = s.GetRequiredService<T1>();
                var p2 = s.GetRequiredService<T2>();
                await taskFactory(p1, p2);
            }));
        }
        public static Task RunAsync<T1, T2, T3>(this IHostBuilder hostBuilder, Func<T1, T2, T3, Task> taskFactory)
        {
            return hostBuilder.RunAsync(new Func<IServiceProvider, Task>(async s =>
            {
                var p1 = s.GetRequiredService<T1>();
                var p2 = s.GetRequiredService<T2>();
                var p3 = s.GetRequiredService<T3>();
                await taskFactory(p1, p2, p3);
            }));
        }
        //public static Task RunAsync<T1, T2, T3,T4>(this IHostBuilder hostBuilder, Func<T1, T2, T3, Task> taskFactory)
        //{
        //    return hostBuilder.RunAsync(new Func<IServiceProvider, Task>(async s =>
        //    {
        //        var p1 = s.GetRequiredService<T1>();
        //        var p2 = s.GetRequiredService<T2>();
        //        var p3 = s.GetRequiredService<T3>();
        //        await taskFactory(p1, p2, p3);
        //    }));
        //}

        // TODO: More parameters

        #endregion


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
            InitializeDependencyContext(host.Services);
            return host;
        }
        public static IServiceProvider InitializeDependencyContext(this IServiceProvider serviceProvider)
        {
            if (LionFireEnvironment.IsMultiApplicationEnvironment)
            {
                DependencyLocatorConfiguration.UseServiceProviderToActivateSingletons = false;
                DependencyLocatorConfiguration.UseSingletons = false;

                if (DependencyContext.AsyncLocal != null) throw new AlreadyException("UNEXPECTED: LionFireEnvironment.IsMultiApplicationEnvironment == true && DependencyContext.AsyncLocal != null "); // FUTURE - deinit on Run complete?  Unit tests running in series?

                DependencyContext.AsyncLocal = new DependencyContext();
                DependencyContext.Current.ServiceProvider = serviceProvider;
            }
            else
            {
                if (DependencyContext.Current == null)
                {
                    DependencyContext.Current = new DependencyContext();
                }

                if (DependencyContext.Current.ServiceProvider == null)
                {
                    DependencyContext.Current.ServiceProvider = serviceProvider;
                }
            }
            return serviceProvider;
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
