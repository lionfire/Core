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
using LionFire.Ontology;
using LionFire.Vos.Collections;
using LionFire.DependencyMachine;

namespace LionFire.Services
{
    public class CollectionTypeFromProperty : ICollectionTypeProvider<VosReference>
    {
        public Type GetCollectionType(VosReference reference)
        {
            return reference.GetVob().AcquireOwn<CollectionType>()?.Type;
        }
    }

    public static class VosServiceExtensions
    {
        public static IHostBuilder AddVos(this IHostBuilder hostBuilder)
            => hostBuilder
                .AddPersisters()
                .ConfigureServices((_, services) =>
                {
                    services
                        .AddSingleton<IDependencyStateMachine, DependencyStateMachine>()
                        .AddSingleton<IRootManager, RootManager>()
                        .AddHostedService<VosInitializationService>()

                        .TryAddEnumerableSingleton<ICollectionTypeProvider<VosReference>, CollectionTypeFromProperty>()

                        .AddSingleton(serviceProvider => serviceProvider.GetService<IOptionsMonitor<VosOptions>>().CurrentValue)

                        .AddSingleton<IPersisterProvider<VosReference>, VosPersisterProvider>()
                        .AddSingleton<IReadHandleProvider<VosReference>, VosHandleProvider>()
                        .AddSingleton<IReadWriteHandleProvider<VosReference>, VosHandleProvider>()
                        .AddSingleton<IWriteHandleProvider<VosReference>, VosHandleProvider>()
                        .AddSingleton(s => s.GetRequiredService<IOptionsMonitor<VosPersisterOptions>>().CurrentValue) // REVIEW - force usage via IOptionsMonitor?
                        .Configure<VosPersisterOptions>(vpo => { })
                        .Configure<VosOptions>(vo =>
                        {
                            //vo.RootNames = new string[] { "", "TestAltRoot" }; // TEMP TEST Alt root

                            //Debug.WriteLine("Configure VosOptions - defaults");
                        })
                        //.PostConfigure<VosOptions>(o =>
                        //{
                        //    //Debug.WriteLine("Mounts:");
                        //    //foreach (var mount in o.Mounts)
                        //    //{
                        //    //    Debug.WriteLine(" - " + mount);
                        //    //}
                        //})

                    
                        .InitializeRootVob((serviceProvider, root) =>
                        {
                            root
                            .AddServiceProvider(s =>
                            {
                                s
                                .AddSingleton(_ => new ServiceDirectory((RootVob)root))
                                .AddSingleton(root)
                                .AddSingleton(root.RootManager)
                                .AddSingleton<VosPersister>()
                                .AddSingleton<VobMounter>()
                                ;
                            }, serviceProvider)
                            //.GetNextRequired<IServiceCollection>()
                            //.AddTransient<IServiceProvider, DynamicServiceProvider>() // Don't want this.  DELETE
                            ;
                        })

                       //.AddVosStores()
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

