using LionFire.Persistence;
using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using LionFire.Persistence.Handles;
using LionFire.Vos.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Persistence.Persisters.Vos;
using Microsoft.Extensions.Options;
using LionFire.Vos.Services;
using LionFire.Vos.Mounts;
using LionFire.DependencyInjection;
using System;

namespace LionFire.Services
{
    public static class VosServiceExtensions
    {
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
                            vo.RootNames = new string[] { "", "TestAltRoot" }; // TEMP TEST Alt root

                            Debug.WriteLine("Configure VosOptions - defaults");
                        })
                        //.PostConfigure<VosOptions>(o =>
                        //{
                        //    //Debug.WriteLine("Mounts:");
                        //    //foreach (var mount in o.Mounts)
                        //    //{
                        //    //    Debug.WriteLine(" - " + mount);
                        //    //}
                        //})

                        .InitializeRootVob(root =>
                        {
                            root
                            .AddServiceProvider(s =>
                            {
                                s
                                //.AddSingleton<Func<IServiceProvider>>(() => new DynamicServiceProvider()) // REVIEW - what is this for?  UNUSED ?
                                .AddSingleton<ServiceDirectory>(_ => new ServiceDirectory((RootVob)root))
                                .AddSingleton<VobMounter>()
                                ;
                            })
                            //.GetNextRequired<IServiceCollection>()
                            //.AddTransient<IServiceProvider, DynamicServiceProvider>() // Don't want this.  DELETE
                            ;
                        })
                       ;

                    // TOTEST - initializing vobs for alternate roots.  MOVE 
                    //  .InitializeVob("InitializeVobTest", vob =>
                    //   {
                    //       vob.AddServiceProvider();
                    //   })
                    //.InitializeVob("TestAltRoot", "/", vob =>
                    //{
                    //    vob.AddServiceProvider();
                    //})
                    //;
                })
            ;

    }
}

