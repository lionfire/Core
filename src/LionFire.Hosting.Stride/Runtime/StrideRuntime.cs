#nullable enable
using LionFire.Stride_.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stride.Core;
using Stride.Core.IO;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Games;
using Stride.Physics;

namespace LionFire.Stride_.Runtime;

public interface ITypedServiceProvider
{
    T? GetService<T>() where T : class;
}

public interface IStrideRuntime : ITypedServiceProvider
{
    ContentManager Content { get; }
    Task Load();
    InheritingServiceRegistry StrideServices { get; }

}

public abstract class StrideRuntime : IStrideRuntime
{
    #region Dependencies

    #region ServiceProvider

    /// <summary>
    /// 
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    // Available through ServiceProvider:
    //
    //ObjectDatabase ObjectDatabase { get; }
    //DatabaseFileProvider DatabaseFileProvider { get; }
    //DatabaseFileProviderService DatabaseFileProviderService { get; }
    //IDatabaseFileProviderService IDatabaseFileProviderService { get; }

    #endregion

    public ILogger Logger { get; }
    public ServiceRegistry GlobalStrideServices { get; }
    //public GlobalContentManager GlobalContent { get; } // FUTURE maybe

    #endregion

    #region Configuration

    protected ContentManagerLoaderSettings loadSettings;
    public string DefaultMainSceneName { get; set; } = "MainScene"; // was "ActionScene";

    #endregion

    #region Lifecycle

    public StrideRuntime(IServiceProvider serviceProvider, ILogger logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;

        GlobalStrideServices = serviceProvider.GetRequiredService<ServiceRegistry>();
        //GlobalContent = serviceProvider.GetService<GlobalContentManager>();

        // Stride's DI container, with global services as fallback for shareable services
        StrideServices = new(GlobalStrideServices);

        // Stride's Content Manager
        Content = new ContentManager(StrideServices);
        StrideServices.AddService<IContentManager>(Content);
        StrideServices.AddService(Content);
        DumpContentIndex();

        // Stride's Game systems
        gameSystems = new GameSystemCollection(StrideServices);
        StrideServices.AddService<IGameSystemCollection>(gameSystems);
        gameSystems.Initialize();

        loadSettings = CreateContentManagerLoaderSettings;

        Load().Wait();
    }

    private void DumpContentIndex()
    {
        int contentCount = 0;
        Logger.LogInformation("Content SearchValues:");

        foreach (var k in Content.FileProvider.ContentIndexMap.SearchValues((_) => true))
        {
            Logger.LogInformation("{key} = {value}", k.Key, k.Value);
        }
        Logger.LogInformation("Content merged Id map:");
        foreach (var k in Content.FileProvider.ContentIndexMap.GetMergedIdMap())
        {
            contentCount++;
            Logger.LogInformation("{key} = {value}", k.Key, k.Value);
        }

        if (contentCount == 0)
        {
            Logger.LogError("Missing Stride Content!");
        }
        DumpContentDatabaseInfo();
    }
    private void DumpContentDatabaseInfo()
    {
        //var ObjectDatabase =ServiceProvider.GetRequiredService<ObjectDatabase>();
        var DatabaseFileProvider = ServiceProvider.GetRequiredService<DatabaseFileProvider>();
        var ObjectDatabase = DatabaseFileProvider.ObjectDatabase;

        Logger.LogInformation("Bundle directory: {bundleDirectory}", ObjectDatabase.BundleBackend.BundleDirectory);


    }

    public async Task Load()
    {
        await LoadMainScene();
    }

    bool isLoaded = false;

    protected virtual async Task LoadMainScene(string? mainSceneName = null)
    {
        if (isLoaded) throw new AlreadyException();
        isLoaded = true;

        mainSceneName ??= DefaultMainSceneName;

        var scene = await Content.LoadAsync<Scene>(mainSceneName, loadSettings);

        SceneInstance = new SceneInstance(StrideServices, scene, ExecutionMode.None);
        //if (SceneInstance.RootScene != null)
        //{
        //    Content.Unload(SceneInstance.RootScene);
        //}
        SceneInstance.RootScene = scene;

        var sceneSystem = new SceneSystem(StrideServices)
        {
            SceneInstance = SceneInstance
        };
        StrideServices.AddService(sceneSystem);


        var Physics = new PhysicsProcessor();
        SceneInstance.Processors.Add(Physics);
    }

    #endregion

    protected virtual ContentManagerLoaderSettings CreateContentManagerLoaderSettings => new ContentManagerLoaderSettings();

    #region State

    public InheritingServiceRegistry StrideServices { get; }

    public ContentManager Content { get; }
    GameSystemCollection gameSystems;
    public SceneInstance? SceneInstance { get; protected set; }
    public PhysicsProcessor Physics { get; protected set; }

    #endregion

    #region ITypedServiceProvider

    public T? GetService<T>()
        where T : class
    {
        var o = StrideServices.GetService<T>();
        if (o != null) return o;
        return ServiceProvider.GetService<T>();
    }

    #endregion
}
