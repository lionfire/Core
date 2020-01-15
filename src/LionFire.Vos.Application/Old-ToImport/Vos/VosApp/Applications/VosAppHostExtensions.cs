#if TODO // OLD - not sure this is needed
using LionFire.Applications.Hosting;
using LionFire.Dependencies;
using LionFire.Execution;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using LionFire.Vos.VosApp;

namespace LionFire.Services
{
    public static class VosAppHostExtensions
    {
        //public static IAppHost AddVos(this IAppHost app) => app.TryAddEnumerableSingleton<IOBus, VosOBus>();

        public static IHostBuilder AddVosApp(this IHostBuilder app, VosAppOptions options = null)
        {
            //app.AddObjectBus<VosOBus>();  // OLD

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
#endif
