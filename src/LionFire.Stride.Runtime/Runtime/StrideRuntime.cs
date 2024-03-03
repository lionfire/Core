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
using Stride.Graphics;
using Stride.Physics;
using Stride.Profiling;
using Stride.Rendering.Fonts;
using Stride.Rendering;
using Stride.Shaders.Compiler;
using Stride.Streaming;
using Stride.VirtualReality;
using System.IO.IsolatedStorage;
using System.Threading;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Stride.Engine.Processors;

namespace LionFire.Stride_.Runtime;

/// <remarks>
/// Based on GameBase.cs
/// </summary>
public abstract class StrideRuntime<TSceneSystem> : IStrideRuntime, ISceneSystem
    where TSceneSystem : SceneSystem
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

    public Action RunCallback { get; set; }
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
        this.StrideServices = new(GlobalStrideServices);

        Content = new ContentManager(this.StrideServices);
        this.StrideServices.AddService<IContentManager>(Content);
        this.StrideServices.AddService(Content);
        DumpContentIndex();

        gameSystems = new GameSystemCollection(this.StrideServices);
        this.StrideServices.AddService<IGameSystemCollection>(gameSystems);

        loadSettings = CreateContentManagerLoaderSettings;

        Script = new ScriptSystem(StrideServices);
        StrideServices.AddService(Script);

        SceneSystem = CreateSceneSystem();
        StrideServices.AddService<SceneSystem>(SceneSystem);

        Bullet2PhysicsSystem = new Bullet2PhysicsSystem(StrideServices);
        StrideServices.AddService<IPhysicsSystem>(Bullet2PhysicsSystem);
        gameSystems.Add(Bullet2PhysicsSystem);
        Bullet2PhysicsSystem.Initialize(); // 

        OnConstructed();
    }

    protected virtual void OnConstructed() { }

    #region Game Systems

    /// <summary>
    /// Gets the script system.
    /// </summary>
    /// <value>The script.</value>
    public ScriptSystem Script { get; }

    #region SceneSystem

    // Upstream PR idea: Ideally there would be a common interface or base class between SceneSystem and HeadlessSceneSystem

    /// <summary>
    /// Gets the scene system.
    /// </summary>
    /// <value>The scene system.</value>
    public TSceneSystem SceneSystem { get; }
    protected abstract TSceneSystem CreateSceneSystem();

    ISceneSystem SceneSystemWrapper => this;

    public SceneInstance? SceneInstance
    {
        get => SceneSystem switch
        {
            SceneSystem ss => ss.SceneInstance,
            //HeadlessSceneSystem hss => hss.SceneInstance,
            _ => null
        };
        protected set
        {
            switch (SceneSystem)
            {
                case SceneSystem ss:
                    ss.SceneInstance = value; break;
                //case HeadlessSceneSystem hss:
                //    hss.SceneInstance = value; break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
    SceneInstance? ISceneSystem.SceneInstance { get => SceneInstance; set => SceneInstance = value; }

    #endregion

    #region Physics

    // TODO: Replace this with IPhysicsSystem
    public Bullet2PhysicsSystem Bullet2PhysicsSystem { get; protected set; }

    #endregion

    #endregion

    protected virtual void AddEarlyGameSystems() { }

    /// <summary>Called after the Game is created, but before <see cref="GraphicsDevice"/> is available and before LoadContent(). Reference page contains code sample.</summary>
    protected virtual void Initialize()
    {
        AddEarlyGameSystems();

        gameSystems.Initialize();

        Content.Serializer.LowLevelSerializerSelector = ParameterContainerExtensions.DefaultSceneSerializerSelector;

        // Add the scheduler system
        // - Must be after Input, so that scripts are able to get latest input
        // - Must be before Entities/Camera/Audio/UI, so that scripts can apply
        // changes in the same frame they will be applied
        GameSystems.Add(Script);

        //// Add the Font system
        //GameSystems.Add(gameFontSystem);
        ////Add the sprite animation System
        //GameSystems.Add(SpriteAnimation);

        //GameSystems.Add(DebugTextSystem);
        //GameSystems.Add(ProfilingSystem);

        //EffectSystem = new EffectSystem(Services);
        //Services.AddService(EffectSystem);

        //// If requested in game settings, compile effects remotely and/or notify new shader requests
        //EffectSystem.Compiler = EffectCompilerFactory.CreateEffectCompiler(Content.FileProvider, EffectSystem, Settings?.PackageName, Settings?.EffectCompilation ?? EffectCompilationMode.Local, Settings?.RecordUsedEffects ?? false);

        //// Setup shader compiler settings from a compilation mode. 
        //// TODO: We might want to provide overrides on the GameSettings to specify debug and/or optim level specifically.
        //if (Settings != null)
        //    EffectSystem.SetCompilationMode(Settings.CompilationMode);

        //GameSystems.Add(EffectSystem);

        //if (Settings != null)
        //Streaming.SetStreamingSettings(Settings.Configurations.Get<StreamingSettings>());
        //GameSystems.Add(Streaming);
        GameSystems.Add(SceneSystem);

        //// Add the Audio System
        //GameSystems.Add(Audio);

        //// Add the VR System
        //GameSystems.Add(VRDeviceSystem);

        //// TODO: data-driven?
        //Content.Serializer.RegisterSerializer(new ImageSerializer());

        Started?.Invoke(this, null);
    }



    /// <summary>
    /// Static event that will be fired when a game is initialized
    /// </summary>
    public static event EventHandler? Started;

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

        SceneSystemWrapper.SceneInstance = SceneInstance;

        SceneInstance.Processors.Add(CreatePhysicsProcessor());
    }

    protected abstract EntityProcessor CreatePhysicsProcessor();

    #endregion

    #region Start

    protected IGamePlatformEx? gamePlatform;
    protected abstract IGamePlatformEx CreateGamePlatform();

    private bool isStarted = false;

    protected virtual void InitGraphics() => throw new NotSupportedException("This runtime does not support graphics.");


    public async Task PrepareToRun(GameContext? context = null)
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
        if (GraphicsDeviceManager != null)
        {
            InitGraphics(); // Tells Context preferred settings from Graphics Device Manager
        }
        gamePlatform = CreateGamePlatform();
    }

   

    private object TickLock = new();

    private void CheckEndRun()
    {
        if (IsExiting && IsRunning && isEndRunRequired)
        {
            EndRun();
            IsRunning = false;
        }
    }

    /// <summary>
    /// Gets or sets the time between each <see cref="Tick"/> when <see cref="IsActive"/> is false.
    /// </summary>
    /// <value>The inactive sleep time.</value>
    public TimeSpan InactiveSleepTime { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is active.
    /// </summary>
    /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Updates the game's clock and calls Update and Draw.
    /// </summary>
    public void Tick()
    {
        lock (TickLock)
        {
            // If this instance is existing, then don't make any further update/draw
            if (IsExiting)
            {
                CheckEndRun();
                return;
            }

            // If this instance is not active, sleep for an inactive sleep time
            if (!IsActive)
            {
                Thread.Sleep(InactiveSleepTime);
                return;
            }

            RawTickProducer();
        }
    }


    private bool forceElapsedTimeToZero;
    private readonly TimeSpan maximumElapsedTime = TimeSpan.FromMilliseconds(500.0);

    /// <summary>
    /// Gets or sets a value indicating whether the elapsed time between each update should be constant,
    /// see <see cref="TargetElapsedTime"/> to configure the duration.
    /// </summary>
    /// <value><c>true</c> if this instance is fixed time step; otherwise, <c>false</c>.</value>
    public bool IsFixedTimeStep { get; set; }

    /// <summary>
    /// Gets or sets the target elapsed time, this is the duration of each tick/update
    /// when <see cref="IsFixedTimeStep"/> is enabled.
    /// </summary>
    /// <value>The target elapsed time.</value>
    public TimeSpan TargetElapsedTime { get; set; }
    private TimeSpan accumulatedElapsedGameTime;

    /// <summary>
    /// Gets or sets a value indicating whether this instance should force exactly one update step per one draw step
    /// </summary>
    /// <value><c>true</c> if this instance forces one update step per one draw step; otherwise, <c>false</c>.</value>
    protected internal bool ForceOneUpdatePerDraw { get; set; }

    /// <summary>
    /// Access to the throttler used to set the minimum time allowed between each updates.
    /// </summary>
    public ThreadThrottler WindowMinimumUpdateRate { get; } = new ThreadThrottler(TimeSpan.FromSeconds(0d));

    /// <summary>
    /// Calls <see cref="RawTick"/> automatically based on this game's setup, override it to implement your own system.
    /// </summary>
    protected virtual void RawTickProducer()
    {
        try
        {
            // Update the timer
            autoTickTimer.Tick();

            var elapsedAdjustedTime = autoTickTimer.ElapsedTimeWithPause;

            if (forceElapsedTimeToZero)
            {
                elapsedAdjustedTime = TimeSpan.Zero;
                forceElapsedTimeToZero = false;
            }

            if (elapsedAdjustedTime > maximumElapsedTime)
            {
                elapsedAdjustedTime = maximumElapsedTime;
            }
            bool drawFrame = true;
            int updateCount = 1;
            var singleFrameElapsedTime = elapsedAdjustedTime;
#if GRAPHICS // non-headless edition
            var drawLag = 0L;

            if (suppressDraw || Window.IsMinimized && DrawWhileMinimized == false)
            {
                drawFrame = false;
                suppressDraw = false;
            }
#endif

            if (IsFixedTimeStep)
            {
                // If the rounded TargetElapsedTime is equivalent to current ElapsedAdjustedTime
                // then make ElapsedAdjustedTime = TargetElapsedTime. We take the same internal rules as XNA
                if (Math.Abs(elapsedAdjustedTime.Ticks - TargetElapsedTime.Ticks) < (TargetElapsedTime.Ticks >> 6))
                {
                    elapsedAdjustedTime = TargetElapsedTime;
                }

                // Update the accumulated time
                accumulatedElapsedGameTime += elapsedAdjustedTime;

                // Calculate the number of update to issue
                if (ForceOneUpdatePerDraw)
                {
                    updateCount = 1;
                }
                else
                {
                    updateCount = (int)(accumulatedElapsedGameTime.Ticks / TargetElapsedTime.Ticks);
                }
#if GRAPHICS // non-headless edition
                if (IsDrawDesynchronized)
                {
                    drawLag = accumulatedElapsedGameTime.Ticks % TargetElapsedTime.Ticks;
                }
                else
#endif
                if (updateCount == 0)
                {
                    drawFrame = false;
                    // If there is no need for update, then exit
                    return;
                }

                // We are going to call Update updateCount times, so we can subtract this from accumulated elapsed game time
                accumulatedElapsedGameTime = new TimeSpan(accumulatedElapsedGameTime.Ticks - (updateCount * TargetElapsedTime.Ticks));
                singleFrameElapsedTime = TargetElapsedTime;
            }

            HeadlessRawTick(singleFrameElapsedTime, updateCount);

#if GRAPHICS
            var window = gamePlatform.MainWindow;
            if (gamePlatform.IsBlockingRun) // throttle fps if Game.Tick() called from internal main loop
            {
                if (window.IsMinimized || window.Visible == false || (window.Focused == false && TreatNotFocusedLikeMinimized))
                {
                    MinimizedMinimumUpdateRate.Throttle(out long _);
                }
                else
                {
                    WindowMinimumUpdateRate.Throttle(out long _);
                }
            }
#else
            if (gamePlatform.IsBlockingRun) // throttle fps if Game.Tick() called from internal main loop
            {
                WindowMinimumUpdateRate.Throttle(out long _);
            }
#endif
        }
        catch (Exception ex)
        {
            Logger.Error("Unexpected exception", ex);
            throw;
        }
    }

    long tickCount = 0;
    /// <summary>
    /// Call this method within your overriden <see cref="RawTickProducer"/> to update and draw the game yourself. <br/>
    /// As this version is manual, there are a lot of functionality purposefully skipped: <br/>
    /// clamping elapsed time to a maximum, skipping drawing when the window is minimized, <see cref="ResetElapsedTime"/>, <see cref="SuppressDraw"/>, <see cref="IsFixedTimeStep"/>, <br/>
    /// <see cref="IsDrawDesynchronized"/>, <see cref="MinimizedMinimumUpdateRate"/> / <see cref="WindowMinimumUpdateRate"/> / <see cref="TreatNotFocusedLikeMinimized"/>.
    /// </summary>
    /// <param name="elapsedTimePerUpdate">
    /// The amount of time passed between each update of the game's system, 
    /// the total time passed would be <paramref name="elapsedTimePerUpdate"/> * <paramref name="updateCount"/>.
    /// </param>
    /// <param name="updateCount">
    /// The amount of updates that will be executed on the game's systems.
    /// </param>
    /// <param name="drawInterpolationFactor">
    /// See <see cref="DrawInterpolationFactor"/>
    /// </param>
    /// <param name="drawFrame">
    /// Draw a frame.
    /// </param>
    protected void HeadlessRawTick(TimeSpan elapsedTimePerUpdate, int updateCount = 1)
    {
        TimeSpan totalElapsedTime = TimeSpan.Zero;
        try
        {
            // Reset the time of the next frame
            for (int i = 0; i < updateCount && !IsExiting; i++)
            {
                tickCount++;
                UpdateTime.Update(UpdateTime.Total + elapsedTimePerUpdate, elapsedTimePerUpdate, true);
                using (Profiler.Begin(GameProfilingKeys.GameUpdate))
                {
                    Update(UpdateTime);
                }
                totalElapsedTime += elapsedTimePerUpdate;
                if (nextTickLog < DateTime.UtcNow)
                {
                    nextTickLog = DateTime.UtcNow + TimeSpan.FromSeconds(10);

                    Logger.LogInformation("Total elapsed: {0},   time per update: {1},   total ticks: {2}, physics max tick: {3}", totalElapsedTime, elapsedTimePerUpdate, tickCount, SceneSystem.SceneInstance.GetProcessor<PhysicsProcessor>()?.Simulation.MaxTickDuration );
                }
            }
        }
        finally
        {
            CheckEndRun();
        }
    }
    DateTime nextTickLog = DateTime.MinValue;


    public async virtual Task Run(GameContext? context = null)
    {
        try
        {
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

    EventHandler<GameUnhandledExceptionEventArgs> UnhandledExceptionInternal;

    /// <summary>
    /// Call this when Run is imminent
    /// - called by GameWindow.InitCallback
    /// - called by StrideRuntime.Run 
    /// </summary>
    protected virtual void OnInitCallback()
    {
        // If/else outside of try-catch to separate user-unhandled exceptions properly
        var unhandledException = UnhandledExceptionInternal;
        if (unhandledException != null)
        {
            // Catch exceptions and transmit them to UnhandledException event
            try
            {
                InitializeBeforeRun();
            }
            catch (Exception e)
            {
                // Some system was listening for exceptions
                unhandledException(this, new GameUnhandledExceptionEventArgs(e, false));
                Exit();
            }
        }
        else
        {
            InitializeBeforeRun();
        }

        #region (local)

        void InitializeBeforeRun()
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

                    // Run an update for the first time
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

        #endregion
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

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        await Load();
        cts.Cancel();
        await PrepareToRun();
        runTask = Run();
        if (gamePlatform!.IsBlockingRun)
        {
            await runTask;
        }

    }
    Task runTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    #endregion
}
