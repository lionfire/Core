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
using System.Linq;

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
                .AddSingleton<IVos, RootManager>()
                .Configure<DependencyMachineConfig>(c =>
                {
                    c.AutoRegisterFromServiceTypes.Add(typeof(IVos));
                });
        }

        public static IHostBuilder AddVos(this IHostBuilder hostBuilder, bool persistence = true)
            => hostBuilder
                .AddPersisters()
                .ConfigureServices((_, services) =>
                {
                    services
                        .AddDependencyMachine()
                        .AddRootManager() // Provides IVos

                    #region TEMP

                        .AddInitializer((Action<IVos>)(vos =>
                        {
                            vos.GetVob("/asdf").Value = "123";
                        }), c => c.Contributes("testAsdf").DependsOn("RootVobs"))
                        .InitializeVob("", null)

                    #endregion

                        .TryAddEnumerableSingleton<ICollectionTypeProvider<VosReference>, CollectionTypeFromVobNode>() // Allows Vobs to provide Collection Type for themselves

                    #region Persistence
                        .If(persistence, s =>
                        {
                            s.AddSingleton<IPersisterProvider<VosReference>, VosPersisterProvider>()
                            .Configure<VosPersisterOptions>(vpo => { })
                            .AddSingleton(s => s.GetRequiredService<IOptionsMonitor<VosPersisterOptions>>().CurrentValue) // REVIEW - force usage via IOptionsMonitor?

                            .AddSingleton<IReadHandleProvider<VosReference>, VosHandleProvider>()
                            .AddSingleton<IReadWriteHandleProvider<VosReference>, VosHandleProvider>()
                            .AddSingleton<IWriteHandleProvider<VosReference>, VosHandleProvider>()
                            ;
                        })
                    #endregion

                      .Configure<VosOptions>(vo =>
                      {
                          //vo.RootNames = new string[] { "", "TestAltRoot" }; // TEMP TEST Alt root
                          //Debug.WriteLine("Configure VosOptions - defaults");
                          vo.ParticipantsFactory = vob =>
                          {
                              return new List<IParticipant>
                                {
                                new Contributor("mounts", $"{vob} mounts") { StartAction = () => vob.InitializeMounts(), },
                                new Participant()
                                {
                                    StartAction = () =>
                                {
                                            vob
                                            .AddServiceProvider(s =>
                                            {
                                                s
                                                .AddSingleton(_ => new ServiceDirectory((RootVob)vob))
                                                .AddSingleton(vob)
                                                .AddSingleton(vob.Root.RootManager)
                                                .AddSingleton<VobMounter>()
                                                .If(persistence, s2=> s2.AddSingleton<VosPersister>())
                                                ;
                                            }, (vob as IHas<IServiceProvider>)?.Object);
                                        //.AddTransient<IServiceProvider, DynamicServiceProvider>() // Don't want this.  DELETE
                                    },
                                    Key = "RootInitializer",
                                }.Contributes("RootVobs"),
                            };
                          };
                      })
                      .AddSingleton(serviceProvider => serviceProvider.GetService<IOptionsMonitor<VosOptions>>().CurrentValue)

                       //.PostConfigure<VosOptions>(o =>
                       //{
                       //    //Debug.WriteLine("Mounts:");
                       //    //foreach (var mount in o.Mounts)
                       //    //{
                       //    //    Debug.WriteLine(" - " + mount);
                       //    //}
                       //})

                       //.TryAddEnumerable(new ServiceDescriptor(typeof(IParticipant), ))
                       //.TryAddEnumerable(new ServiceDescriptor(typeof(VobInitializer), ))

                       //.InitializeVob("/_/RootInitializer", v=>v.Value = )



                       //.AddNamedSingleton<VobInitializer>("RootInitializer", new VobInitializer()
                       //{
                       //    Contributes = new List<object> { "RootVobs" },
                       //    StartAction = async (ctx, ct) =>
                       //    {
                       //        await Task.Delay(0);
                       //        return null;
                       //    },
                       //})

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

