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
using LionFire.Services.DependencyMachines;
using System.Runtime.CompilerServices;
using LionFire.Hosting;

namespace LionFire.Services
{
    public class CollectionTypeFromVobNode : ICollectionTypeProvider<VobReference>
    {
        public Type GetCollectionType(VobReference reference) => reference.GetVob().AcquireOwn<CollectionType>()?.Type;
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

        public static IHostBuilder AddVos(this IHostBuilder hostBuilder, bool persistence = true, bool enableLogging = true)
            => hostBuilder
                .AddPersisters()
                .ConfigureServices((_, services) =>
                {
                    services
                        .AddDependencyMachine()
                        .AddRootManager() // Provides IVos

                        //#region TEMP

                        //    .AddInitializer((Action<IVos>)(vos =>
                        //    {
                        //        vos.GetVob("/asdf").Value = "123";
                        //    }), c => c.Contributes("testAsdf").DependsOn("RootVobs"))
                        //    .InitializeVob("", null)

                        //#endregion

                        .TryAddEnumerableSingleton<ICollectionTypeProvider<VobReference>, CollectionTypeFromVobNode>() // Allows Vobs to provide Collection Type for themselves

                    #region Persistence
                        .If(persistence, s =>
                        {
                            s.AddSingleton<IPersisterProvider<VobReference>, VosPersisterProvider>()
                            .Configure<VosPersisterOptions>(vpo => { })
                            .AddSingleton(s => s.GetRequiredService<IOptionsMonitor<VosPersisterOptions>>().CurrentValue) // REVIEW - force usage via IOptionsMonitor?

                            .AddSingleton<IReadHandleProvider<VobReference>, VosHandleProvider>()
                            .AddSingleton<IReadWriteHandleProvider<VobReference>, VosHandleProvider>()
                            .AddSingleton<IWriteHandleProvider<VobReference>, VosHandleProvider>()
                            ;
                        })
                    #endregion

                      .Configure<VosOptions>(vo =>
                      {
                          // FIXME: PrimaryRootInitializers should be a collection of Func<IServiceProvider, IParticipant> ??

                          //vo.RootNames = new string[] { "", "TestAltRoot" }; // TEMP TEST Alt root
                          //Debug.WriteLine("Configure VosOptions - defaults");

                          vo.PrimaryRootInitializers.Add(vobRoot => // Initializers for the Primary root
                          {
                              var vobRootChain = new List<string>
                              {
                                  "services:",
                                  "environment:",
                                  "mounts:",
                                  "", // Vob itself
                              };
                              var list = new List<IParticipant>();

                              list.AddRange(DependencyStages.CreateStageChain(new string[] { "vos:" }.Concat(vobRootChain.Select(c => c + vobRoot.Reference)).ToArray()));

                              list.Add(new Participant()
                              {
                                  Key = "RootInitializer",
                                  StartAction = () =>
                                  {
                                      vobRoot
                                        .AddServiceProvider(s =>
                                        {
                                            s
                                            .AddSingleton(_ => new ServiceDirectory((RootVob)vobRoot))
                                            .AddSingleton(vobRoot)
                                            .AddSingleton(vobRoot.Root.RootManager)
                                            .AddSingleton<VobMounter>()
                                            .If(persistence, s2 => s2.AddSingleton<VosPersister>())
                                            ;
                                        }, (vobRoot as IHas<IServiceProvider>)?.Object);
                                      //.AddTransient<IServiceProvider, DynamicServiceProvider>() // Don't want this.  DELETE
                                  },
                              }.Contributes("services:" + vobRoot.Reference /* vos:/ */)); // "RootVobs"),

                              //new Dependency(VosInitStages.RootMountStage(vobRoot.Name), $"{vobRoot} mounts") { StartAction = () => vobRoot.InitializeMounts(), }.DependsOn("vos:"),
                              list.Add(new Participant(key: VosInitStages.RootMountStage(vobRoot.Name)) { StartAction = () => vobRoot.InitializeMounts(), }.After("environment:" + vobRoot.Reference.ToString()).Contributes(vobRoot.Reference.ToString())); 
                              return list;
                          });
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

