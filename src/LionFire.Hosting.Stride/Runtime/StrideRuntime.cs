#nullable enable
using LionFire.Stride_.Core;
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stride.Core;
using Stride.Core.Diagnostics;
using Stride.Core.IO;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Games;
using Stride.Games.Time;
using Stride.Physics;
using System.IO.IsolatedStorage;

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

[Flags]
public enum ClientOrServer : byte
{
    Unspecified = 0,
    Client = 1 << 1,
    Server = 1 << 2,
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

    protected abstract ClientOrServer ClientOrServer { get; }

    protected ContentManagerLoaderSettings loadSettings;
    public string DefaultMainSceneName { get; set; } = "MainScene"; // was "ActionScene";

    public AppContextType AppContextType
    {
        get
        {
            AppContextType c;
            if (OperatingSystem.IsWindows())
                c = AppContextType.Desktop;
            else if (OperatingSystem.IsAndroid())
                c = AppContextType.Android;
            else if (OperatingSystem.IsIOS() || OperatingSystem.IsTvOS() || OperatingSystem.IsWatchOS())
                c = AppContextType.iOS;
            else
                c = AppContextType.DesktopSDL;
            return c;
        }
    }
    #region Derived

    GameContext? gameContext;
    public bool IsClient => ClientOrServer == ClientOrServer.Client;
    public bool IsServer => ClientOrServer == ClientOrServer.Server;

    #endregion

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
    }

    private void DumpContentIndex()
    {
        int contentCount = 0;
        Logger.LogTrace("Content SearchValues:");

        foreach (var k in Content.FileProvider.ContentIndexMap.SearchValues((_) => true))
        {
            Logger.LogTrace("{key} = {value}", k.Key, k.Value);
        }
        Logger.LogTrace("Content merged Id map:");
        foreach (var k in Content.FileProvider.ContentIndexMap.GetMergedIdMap())
        {
            contentCount++;
            Logger.LogTrace("{key} = {value}", k.Key, k.Value);
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

    #region Load

    private bool isLoaded = false;
    public async Task Load()
    {
        if (isLoaded) return;
        isLoaded = true; // TODO: guard against multiple calls
        await LoadMainScene();
    }

    protected virtual async Task LoadMainScene(string? mainSceneName = null)
    {
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

    #region Start

    IGamePlatformEx gamePlatform;
    protected abstract IGamePlatformEx CreateGamePlatform();

    private bool isStarted = false;
    public async Task Run(GameContext? context = null)
    {
        if (isStarted) throw new AlreadyException();
        isStarted = true;
        await Load();

        GraphicsDeviceManager = StrideServices.GetService<IGraphicsDeviceManager>();
        if (GraphicsDeviceManager == null)
        {
            if (IsClient)
            {
                throw new InvalidOperationException("No GraphicsDeviceManager found");
            }
        }

        Context = context ??= GameContextFactory.NewGameContext(AppContextType);
        PrepareContext();

        try
        {
            if (GraphicsDeviceManager != null) {
                // TODO temporary workaround as the engine doesn't support yet resize
                var graphicsDeviceManagerImpl = (GraphicsDeviceManager)GraphicsDeviceManager;
                throw new NotImplementedException("TODO: Init GameContext");
#if TODO
                Context.RequestedWidth = graphicsDeviceManagerImpl.PreferredBackBufferWidth;
                Context.RequestedHeight = graphicsDeviceManagerImpl.PreferredBackBufferHeight;
                Context.RequestedBackBufferFormat = graphicsDeviceManagerImpl.PreferredBackBufferFormat;
                Context.RequestedDepthStencilFormat = graphicsDeviceManagerImpl.PreferredDepthStencilFormat;
                Context.RequestedGraphicsProfile = graphicsDeviceManagerImpl.PreferredGraphicsProfile;
                Context.DeviceCreationFlags = graphicsDeviceManagerImpl.DeviceCreationFlags;
#endif
            }
            gamePlatform = CreateGamePlatform();
            gamePlatform.Run(Context);

            if (gamePlatform.IsBlockingRun)
            {
                // If the previous call was blocking, then we can call Endrun
                EndRun();
            }
            else
            {
                // EndRun will be executed on Game.Exit
                isEndRunRequired = true;
            }
        }
        finally
        {
            if (!isEndRunRequired)
            {
                IsRunning = false;
            }
        }
    }

    protected virtual void PrepareContext()
    {

    }


    private bool isEndRunRequired;

    /// <summary>
    /// Called after all components are initialized, before the game loop starts.
    /// </summary>
    protected virtual void BeginRun()
    {
    }

    protected virtual void EndRun()
    {
    }

    /// <summary>
    /// Gets a value indicating whether this instance is exiting.
    /// </summary>
    /// <value><c>true</c> if this instance is exiting; otherwise, <c>false</c>.</value>
    public bool IsExiting { get; private set; }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        IsExiting = true;
        gamePlatform.Exit();
    }

    #endregion

    internal void InitializeBeforeRun()
    {
        try
        {
            using (var profile = Profiler.Begin(GameProfilingKeys.GameInitialize))
            {
#if TODO
                // Initialize this instance and all game systems before trying to create the device.
                Initialize();

                // Make sure that the device is already created
                graphicsDeviceManager.CreateDevice();

                // Gets the graphics device service
                graphicsDeviceService = Services.GetService<IGraphicsDeviceService>();
                if (graphicsDeviceService == null)
                {
                    throw new InvalidOperationException("No GraphicsDeviceService found");
                }

                // Checks the graphics device
                if (graphicsDeviceService.GraphicsDevice == null)
                {
                    throw new InvalidOperationException("No GraphicsDevice found");
                }

                // Setup the graphics device if it was not already setup.
                SetupGraphicsDeviceEvents();

                // Bind Graphics Context enabling initialize to use GL API eg. SetData to texture ...etc
                BeginDraw();

                LoadContentInternal();
#endif
                IsRunning = true;

                BeginRun();

                autoTickTimer.Reset();
                UpdateTime.Reset(UpdateTime.Total);

                // Run the first time an update
                using (Profiler.Begin(GameProfilingKeys.GameUpdate))
                {
                    Update(UpdateTime);
                }
#if TODO
                // Unbind Graphics Context without presenting
                EndDraw(false);
#endif
            }
        }
        catch (Exception ex)
        {
            Log.Error("Unexpected exception", ex);
            throw;
        }
    }
  
    #region IsRunning

    /// <summary>
    /// Gets a value indicating whether this instance is running.
    /// </summary>
    public bool IsRunning { get; private set; }
    //public bool IsRunning => isStarted && !isFinished;

    #endregion

    #region Finished

    private bool isFinished = false;

    #endregion

#endregion

    protected virtual ContentManagerLoaderSettings CreateContentManagerLoaderSettings => new ContentManagerLoaderSettings();

    #region State

    #region Time

    private readonly TimerTick autoTickTimer = new();
    /// <summary>
    /// The total and delta time to be used for logic running in the update loop.
    /// </summary>
    public GameTime UpdateTime { get; }

    #endregion

    public GameContext? Context { get; protected set; }

    public InheritingServiceRegistry StrideServices { get; }

    public ContentManager Content { get; }

    public GameSystemCollection GameSystems => gameSystems;
    GameSystemCollection gameSystems;
    public SceneInstance? SceneInstance { get; protected set; }
    public PhysicsProcessor Physics { get; protected set; }
    public IGraphicsDeviceManager? GraphicsDeviceManager { get; protected set; }

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

