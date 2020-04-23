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
using LionFire.DependencyMachines;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LionFire.Services
{
    public class CollectionTypeFromVobNode : ICollectionTypeProvider<VosReference>
    {
        public Type GetCollectionType(VosReference reference)
        {
            return reference.GetVob().AcquireOwn<CollectionType>()?.Type;
        }
    }

    public static class VosServiceExtensions
    {
        public static IServiceCollection AddRootManager(this IServiceCollection services)
        {
            return services
                .AddSingleton<IRootManager, RootManager>()
                .Configure<DependencyMachineConfig>(c =>
                {
                    c.AutoRegisterFromServiceTypes.Add(typeof(IRootManager));
                });
        }

        public static IHostBuilder AddVos(this IHostBuilder hostBuilder)
            => hostBuilder
                .AddPersisters()
                .ConfigureServices((_, services) =>
                {
                    services
                        .AddDependencyMachine()

                        .Configure<VosOptions>(vo =>
                        {
                            //vo.RootNames = new string[] { "", "TestAltRoot" }; // TEMP TEST Alt root

                            //Debug.WriteLine("Configure VosOptions - defaults");
                        })
                        .AddSingleton(serviceProvider => serviceProvider.GetService<IOptionsMonitor<VosOptions>>().CurrentValue)

                        .AddRootManager()
                        //.AddHostedService<VosInitializationService>()


                        .TryAddEnumerableSingleton<ICollectionTypeProvider<VosReference>, CollectionTypeFromVobNode>()

                        .AddSingleton<IPersisterProvider<VosReference>, VosPersisterProvider>()
                        .Configure<VosPersisterOptions>(vpo => { })
                        .AddSingleton(s => s.GetRequiredService<IOptionsMonitor<VosPersisterOptions>>().CurrentValue) // REVIEW - force usage via IOptionsMonitor?

                        .AddSingleton<IReadHandleProvider<VosReference>, VosHandleProvider>()
                        .AddSingleton<IReadWriteHandleProvider<VosReference>, VosHandleProvider>()
                        .AddSingleton<IWriteHandleProvider<VosReference>, VosHandleProvider>()

                        //.PostConfigure<VosOptions>(o =>
                        //{
                        //    //Debug.WriteLine("Mounts:");
                        //    //foreach (var mount in o.Mounts)
                        //    //{
                        //    //    Debug.WriteLine(" - " + mount);
                        //    //}
                        //})

                        .TryAddEnumerable(new ServiceDescriptor(typeof(IParticipant), ))
                        .TryAddEnumerable(new ServiceDescriptor(typeof(VobInitializer), ))

                        .AddNamedSingleton<VobInitializer>("RootInitializer", new VobInitializer()
                        {
                            Contributes = new List<object> { "RootVobs" },
                            StartAction = async (ctx, ct) =>
                            {
                                await Task.Delay(0);
                                return null;
                            },
                        })
                        .InitializeVob("/".ToVosReference(), (serviceProvider, vob) =>
                        //.InitializeRootVob((serviceProvider, root) =>
                        {
                            vob
                            .AddServiceProvider(s =>
                            {
                                s
                                .AddSingleton(_ => new ServiceDirectory((RootVob)vob))
                                .AddSingleton(vob)
                                .AddSingleton(vob.Root.RootManager)
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

