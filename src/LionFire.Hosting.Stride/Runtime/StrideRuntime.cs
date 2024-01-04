#nullable enable
using LionFire.Stride_.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stride.Core;
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
    public string DefaultMainSceneName { get; set; } = "MainScene";

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

        // Stride's Game systems
        gameSystems = new GameSystemCollection(StrideServices);
        StrideServices.AddService<IGameSystemCollection>(gameSystems);
        gameSystems.Initialize();

        loadSettings = CreateContentManagerLoaderSettings;

    }

    public async Task Load()
    {
        await LoadMainScene();
    }

    protected virtual async Task LoadMainScene(string? mainSceneName = null)
    {
        mainSceneName ??= DefaultMainSceneName;

        var scene = await Content.LoadAsync<Scene>(mainSceneName, loadSettings);

        SceneInstance = new SceneInstance(StrideServices, scene, ExecutionMode.None);
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
