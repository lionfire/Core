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
using LionFire.DependencyMachines;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using LionFire.Services;
using LionFire.Referencing;
using OpenTelemetry.Metrics;

namespace LionFire.Hosting;

public static class VosHostingX
{
    #region ILionFireHostBuilder

    public static ILionFireHostBuilder Vos(this ILionFireHostBuilder hostBuilder, bool persistence = true, bool enableLogging = true)
    {
        hostBuilder
            .Persisters()
            .ForHostBuilder(builder => builder
                .AddVos(persistence, enableLogging))
        ;
        return hostBuilder;
    }

    #endregion

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
                    
                    .AddSingleton<PersisterEvents, PersisterEvents>()
                    //.TryAddEnumerableSingleton<IAttacher<VosPersister>, ArchiveAdapter>()

                    //#region TEMP

                    //    .AddInitializer((Action<IVos>)(vos =>
                    //    {
                    //        vos.GetVob("/asdf").Value = "123";
                    //    }), c => c.Contributes("testAsdf").DependsOn("RootVobs"))
                    //    .InitializeVob("", null)

                    //#endregion

                    .TryAddEnumerableSingleton<ICollectionTypeProvider<VobReference>, CollectionTypeFromVobNode>() // Allows Vobs to provide Collection Type for themselves - REVIEW
                #region Referencing

                    .TryAddEnumerableSingleton<IReferenceProvider, VobReferenceProvider>()
                    .TryAddEnumerableSingleton<IReferenceProvider, TypedVobReferenceProvider>()

                #endregion

                #region Persistence
                    .If(persistence, s =>
                    {
                        s
                        .AddSingleton<IPersisterProvider<IVobReference>, VosPersisterProvider>()
                        .AddSingleton<IPersister<IVobReference>, VosPersister>()
                        .Configure<VosPersisterOptions>(vpo => { })
                        .AddSingleton(s => s.GetRequiredService<IOptionsMonitor<VosPersisterOptions>>().CurrentValue) // REVIEW - force usage via IOptionsMonitor?

                        .AddSingleton<IReadHandleProvider<IVobReference>, VosHandleProvider>()
                        .AddSingleton<IReadWriteHandleProvider<IVobReference>, VosHandleProvider>()
                        .AddSingleton<IWriteHandleProvider<IVobReference>, VosHandleProvider>()
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

    public static MeterProviderBuilder AddVosInstrumentation(this MeterProviderBuilder b) => b
        .AddMeter("LionFire.Vos")
        .AddMeter("LionFire.Vos.RootManager")
        .AddPersistenceInstrumentation()
        .AddPersistersInstrumentation() 
        ;

    public static MeterProviderBuilder AddPersistersInstrumentation(this MeterProviderBuilder b) => b // MOVE
                .AddMeter("LionFire.Persisters.SharpZipLib")
                .AddMeter("LionFire.Persisters.SharpZipLib.SharpZipLibExpander")
        ;
    public static MeterProviderBuilder AddPersistenceInstrumentation(this MeterProviderBuilder b) => b // MOVE
            .AddMeter("LionFire.Persistence.Filesystem")
            .AddMeter("LionFire.Persistence.Handles.WeakHandleRegistry")
    ;
}

