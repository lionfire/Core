// Inspired by and derived from Stride's GameBase.cs
//
// Stride license:
//
// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//
// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
using System.Threading;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace LionFire.Stride_.Runtime;

/// <remarks>
/// Based on GameBase.cs
/// </summary>
public abstract class StrideRuntime : IStrideRuntime
{

    #region Dependencies

    #region ServiceProvider

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Available through ServiceProvider:
    ///
    /// ObjectDatabase ObjectDatabase { get; }
    /// DatabaseFileProvider DatabaseFileProvider { get; }
    /// DatabaseFileProviderService DatabaseFileProviderService { get; }
    /// IDatabaseFileProviderService IDatabaseFileProviderService { get; }
    /// 
    /// </remarks>
    public IServiceProvider ServiceProvider { get; }

    //ObjectDatabase ObjectDatabase  => ServiceProvider.GetRequiredService<ObjectDatabase>();
    /// DatabaseFileProvider DatabaseFileProvider => ServiceProvider.GetRequiredService<DatabaseFileProvider>();
    /// DatabaseFileProviderService DatabaseFileProviderService => ServiceProvider.GetRequiredService<DatabaseFileProviderService>();
    /// IDatabaseFileProviderService IDatabaseFileProviderService => ServiceProvider.GetRequiredService<IDatabaseFileProviderService>();

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

    IGamePlatformEx? gamePlatform;
    protected abstract IGamePlatformEx CreateGamePlatform();

    private bool isStarted = false;

    protected virtual void InitGraphics() => throw new NotSupportedException("This runtime does not support graphics.");

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
            if (GraphicsDeviceManager != null)
            {
                InitGraphics(); // Tells Context preferred settings from Graphics Device Manager
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


    /// <summary>Called after the Game is created, but before <see cref="GraphicsDevice"/> is available and before LoadContent(). Reference page contains code sample.</summary>
    protected virtual void Initialize()
    {
        GameSystems.Initialize();
    }

    internal virtual void LoadContentInternal()
    {
        GameSystems.LoadContent();
    }

    /// <summary>
    /// Called after all components are initialized, before the game loop starts.
    /// </summary>
    protected virtual void BeginRun()
    {
    }

    protected virtual void InitializeGraphicsBeforeRun() { }
    internal void InitializeBeforeRun()
    {
        try
        {
            using (var profile = Profiler.Begin(GameProfilingKeys.GameInitialize))
            {
                // Initialize this instance and all game systems before trying to create the device.
                Initialize();

                InitializeGraphicsBeforeRun();

                LoadContentInternal();
                IsRunning = true;

                BeginRun();

                autoTickTimer.Reset();
                UpdateTime.Reset(UpdateTime.Total);

                // Run the first time an update
                using (Profiler.Begin(GameProfilingKeys.GameUpdate))
                {
                    Update(UpdateTime);
                }

                // Unbind Graphics Context without presenting
                EndDraw(false);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected exception");
            throw;
        }
    }

    protected virtual void EndRun()
    {
    }

    private bool isEndRunRequired;

    #endregion

    #region IsRunning

    /// <summary>
    /// Gets a value indicating whether this instance is running.
    /// </summary>
    public bool IsRunning { get; private set; }
    //public bool IsRunning => isStarted && !isFinished;

    #endregion

    #region Update

    /// <summary>
    /// Reference page contains links to related conceptual articles.
    /// </summary>
    /// <param name="gameTime">
    /// Time passed since the last call to Update.
    /// </param>
    protected virtual void Update(GameTime gameTime)
    {
        GameSystems.Update(gameTime);
    }

    #endregion

    #region Finished

    private bool isFinished = false;

    #endregion

    #region Exit

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

    #endregion

    #region Graphics

    protected virtual void EndDraw(bool present) { }

    #endregion

    protected virtual ContentManagerLoaderSettings CreateContentManagerLoaderSettings => new ContentManagerLoaderSettings();

    #region State

    #region Time

    private readonly TimerTick autoTickTimer = new();
    /// <summary>
    /// The total and delta time to be used for logic running in the update loop.
    /// </summary>
    public GameTime UpdateTime { get; } = new();

    #endregion

    public GameContext? Context { get; protected set; }

    public InheritingServiceRegistry StrideServices { get; }

    public ContentManager Content { get; }

    public GameSystemCollection GameSystems => gameSystems;
    GameSystemCollection gameSystems;
    public SceneInstance? SceneInstance { get; protected set; }
    public PhysicsProcessor? Physics { get; protected set; }
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

    #region IHostedService

    public CancellationToken IsStarted => cts.Token;
    CancellationTokenSource cts = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Load();
        cts.Cancel();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    #endregion
}

