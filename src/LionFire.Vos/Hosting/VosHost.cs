using System;
using LionFire.Persistence;
using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;
using System.Diagnostics;
using LionFire.Persistence.Handles;
using LionFire.Vos.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Persistence.Persisters.Vos;
using Microsoft.Extensions.Options;
using LionFire.Vos.Services;
using LionFire.Vos.Mounts;

namespace LionFire.Services
{
    public static class VosHost
    {
     
        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true, Action<IServiceCollection> serializers = null)
        {
            return PersistersHost.Create(args, defaultBuilder: defaultBuilder)
                .AddVos();
        }

        public static IHostBuilder AddVos(this IHostBuilder hostBuilder)
            => hostBuilder
                .AddPersisters()
                .ConfigureServices((_, services) =>
                {
                    services
                        .AddSingleton<VosRootManager>()
                        .AddSingleton<VosInitializer>()
                        .AddSingleton(serviceProvider => serviceProvider.GetService<IOptionsMonitor<VosOptions>>().CurrentValue)
                        .AddSingleton<IPersisterProvider<VosReference>, VosPersisterProvider>()
                        .AddSingleton<IReadHandleProvider<VosReference>, VosHandleProvider>()
                        .AddSingleton<IReadWriteHandleProvider<VosReference>, VosHandleProvider>()
                        .Configure<VosOptions>(vo =>
                        {
                            vo.RootNames = new string[] { "", "TestAltRoot" };

                            Debug.WriteLine("Configure VosOptions - defaults");
                        })
                        .PostConfigure<VosOptions>(o =>
                        {
                            Debug.WriteLine("Mounts:");
                            foreach (var mount in o.Mounts)
                            {
                                Debug.WriteLine(" - " + mount);
                            }
                        })

                        // TEMP - MOVE this.  Work in progress:
                        .InitializeRootVob(vob =>
                        {
                            vob
                            .AddServiceProvider()
                            .GetService<IServiceCollection>()
                                .AddSingleton<VobMounter>()
                            ;

                        })
                          .InitializeVob("InitializeVobTest", vob =>
                          {
                              vob.AddServiceProvider();
                          })
                        .InitializeVob("TestAltRoot", "/", vob =>
                        {
                            vob.AddServiceProvider();
                        })
                    ;
                })
            ;

    }
}

