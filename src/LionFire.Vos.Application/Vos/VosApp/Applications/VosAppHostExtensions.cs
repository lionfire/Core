using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Applications.Hosting;
using LionFire.DependencyInjection;
using LionFire.Execution;
using LionFire.ObjectBus;
using LionFire.Vos;
using LionFire.Vos.Assets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Internal;

namespace LionFire.Hosting.ExtensionMethods
{
    public class InitializingLifetimeWrapper<T> : IHostLifetime
        where T : IHostLifetime
    {
        IServiceProvider serviceProvider;
        T WrappedLifetime;
        IEnumerable<IInitializable3> initializers;
        //public IHostLifetime WrappedLifetime { get; set; }


        public InitializingLifetimeWrapper(T wrappedLifetime, IEnumerable<IInitializable3> initializers) {
            //this.serviceProvider = serviceProvider;
            this.WrappedLifetime = wrappedLifetime;// ?? new TValue();
            this.initializers = initializers;
        }

        //public InitializingLifetimeWrapper(IHostLifetime wrapped = null) { WrappedLifetime = wrapped ?? new ConsoleLifetime(); }

        public async Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            if (initializers != null)
            {
                await initializers.RepeatAllUntilNull(i => i.Initialize, cancellationToken);
            }
            await WrappedLifetime.WaitForStartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await WrappedLifetime.StopAsync(cancellationToken);
        }
    }

    public static class VosAppHostExtensions
    {
        //public static IAppHost AddVos(this IAppHost app) => app.TryAddEnumerableSingleton<IOBus, VosOBus>();

        public static IHostBuilder AddVosApp(this IHostBuilder app, VosAppOptions options = null)
        {
            app.AddObjectBus<VosOBus>();

            //var va = new VosApp(options);
            //app.Add(va);

            app.ConfigureServices((c, s) =>
            {
                s
                .AddSingleton<IHostLifetime, InitializingLifetimeWrapper<ConsoleLifetime>>()
                .AddSingleton<IVosContextResolver, DefaultVosContextResolver>()
                .AddSingleton<VosApp>()
                .TryAddEnumerableSingleton<IInitializable3, VosApp>()
                ;
                //s.AddInit(async a => await a.GetService<VosApp>().Initialize());
            });

            app.UseVosAppForAssets();

            return app;
        }

        /// <summary>
        /// TODO REVIEW - does or should more need to be done here to wire up Assets to Vos??
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IHostBuilder UseVosAppForAssets(this IHostBuilder app)
        {
            VosAssetsSettings.DefaultPathFromNameForType = VosApp.GetAssetPath;

            return app;
        }
    }
}
