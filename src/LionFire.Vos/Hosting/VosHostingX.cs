using LionFire.FlexObjects;
using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Persistence.Handles;
using LionFire.Vos.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Persistence.Persisters.Vos;
using Microsoft.Extensions.Options;
using LionFire.Vos.Services;
using LionFire.Vos.Mounts;
using LionFire.DependencyMachines;
using LionFire.Services;
using LionFire.Referencing;
using OpenTelemetry.Metrics;
using LionFire.FlexObjects.Services;

namespace LionFire.Hosting;

public static class VosHostingX
{
    #region ILionFireHostBuilder

    public static ILionFireHostBuilder Vos(this ILionFireHostBuilder hostBuilder, bool persistence = true)
    {
        hostBuilder
            .Persisters()
            .ForHostBuilder(builder => builder
                .AddVos(persistence)
                .AddVosPersistence()
                )
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

    internal static IHostBuilder AddVosPersistence(this IHostBuilder hostBuilder)
        => hostBuilder.ConfigureServices(s => s
            .AddSingleton<IPersisterProvider<IVobReference>, VosPersisterProvider>()
            .AddSingleton<IPersister<IVobReference>, VosPersister>()
            .Configure<VosPersisterOptions>(vpo => { })
            .AddSingleton(s => s.GetRequiredService<IOptionsMonitor<VosPersisterOptions>>().CurrentValue) // REVIEW - force usage via IOptionsMonitor?

            .AddSingleton<IReadHandleProvider<IVobReference>, VosHandleProvider>()
            .AddSingleton<IReadWriteHandleProvider<IVobReference>, VosHandleProvider>()
            .AddSingleton<IWriteHandleProvider<IVobReference>, VosHandleProvider>()

            .Configure<VosOptions>(vo =>
            {
                vo.PrimaryRootInitializers.Add(vobRoot => // Initializers for the Primary root
                {
                    var list = new List<IParticipant>
                    {
                        new Participant()
                        {
                            Key = "VosPersistence",
                            StartAction = () =>
                            {
                                vobRoot.Query<IServiceCollection>()!.AddSingleton<VosPersister>();
                            },
                        }
                        .DependsOn("services:" + vobRoot.Reference + $"<{typeof(FlexServiceProvider).Name}>")
                        .DependsOn("services:" + vobRoot.Reference + $"<{typeof(IServiceCollection).Name}>")
                        .Contributes("services:" + vobRoot.Reference + $"<{typeof(VosPersister).Name}>")
                    };

                    return list;
                });
            })
        );

    public static IHostBuilder AddVos(this IHostBuilder hostBuilder, bool persistence = true)
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
                                    .AddDynamicServiceProvider(s =>
                                    {
                                        s
                                        .AddSingleton(_ => new ServiceDirectory((RootVob)vobRoot)) // OLD
                                        .AddSingleton(vobRoot)
                                        .AddSingleton(vobRoot.Root.RootManager)
                                        .AddSingleton<VobMounter>()
                                        //.If(persistence, s2 => s2.AddSingleton<VosPersister>()) // MOVE to AddVosPersistence
                                        ;
                                    });
                              },
                          }
                              .Contributes("services:" + vobRoot.Reference /* vos:/ */)
                              .Contributes("services:" + vobRoot.Reference + $"<{typeof(VobMounter).Name}>") // ENH: .ContributesService<>()
                              .Contributes("services:" + vobRoot.Reference + $"<{typeof(IServiceProvider).Name}>")
                              .Contributes("VobNode:" + vobRoot.Reference + $"<{typeof(IServiceProvider).Name}>") // REVIEW: it get set on Flex and as VobNode                              
                              .Contributes("services:" + vobRoot.Reference + $"<{typeof(FlexServiceProvider).Name}>")
                              .Contributes("services:" + vobRoot.Reference + $"<{typeof(IServiceCollection).Name}>")
                              .Contributes("services:" + vobRoot.Reference + $"<{typeof(IRootVob).Name}>")
                              .Contributes("services:" + vobRoot.Reference + $"<{typeof(IVos).Name}>")
                          );

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

