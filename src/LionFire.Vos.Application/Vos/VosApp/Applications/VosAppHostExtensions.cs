using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Applications.Hosting;
using LionFire.DependencyInjection;
using LionFire.Execution;
using LionFire.ObjectBus;
using LionFire.Vos;
using LionFire.Vos.Assets;

namespace LionFire.Applications.Hosting
{
    public static class VosAppHostExtensions
    {
        public static IAppHost AddVos(this IAppHost app) => app.TryAddEnumerableSingleton<IOBus, VosOBus>();

        public static IAppHost AddVosApp(this IAppHost app, VosAppOptions options = null)
        {
            app.AddVos();

            app.AddSingleton<IVosContextResolver, DefaultVosContextResolver>();

            {
                var va = new VosApp(options);
                app.Add(va);
                app.AddSingleton<VosApp>(va);
            }

            app.UseVosAppForAssets();

            app.AddInit(a => a.GetService<VosApp>().Initialize());

            return app;
        }

        /// <summary>
        /// TODO REVIEW - does or should more need to be done here to wire up Assets to Vos??
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IAppHost UseVosAppForAssets(this IAppHost app)
        {
            VosAssetsSettings.DefaultPathFromNameForType = VosApp.GetAssetPath;

            return app;
        }
    }
}
