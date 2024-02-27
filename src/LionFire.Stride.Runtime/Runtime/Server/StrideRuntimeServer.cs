using Stride.Core.Serialization.Contents;
using Stride.Engine.Design;
using Stride.Engine;
using Stride.Physics;
using Microsoft.Extensions.DependencyInjection;
using Stride.Games;
using Stride.Data;
using HeadlessPhysicsProcessor = Stride.Physics.PhysicsProcessor;

namespace LionFire.Stride_.Runtime;

public static class StrideServerConfig
{
    /// <summary>
    /// Number of simulations to run per second.
    /// </summary>
    public const int PhysicsSimulationRate = 30;

    // Note: do not use TimeSpan.FromSeconds because it is less precise in calculating the time for some reason.
    public static readonly TimeSpan PhysicsFixedTimeStep = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / PhysicsSimulationRate);
}

public class StrideRuntimeServer : StrideRuntime<HeadlessSceneSystem>, IGameSettingsService
{

    protected override ClientOrServer ClientOrServer => ClientOrServer.Server;

    public StrideRuntimeServer(IServiceProvider serviceProvider, ILogger<StrideRuntimeServer> logger) : base(serviceProvider, logger)
    {
    }

    protected override HeadlessSceneSystem CreateSceneSystem() => new HeadlessSceneSystem(StrideServices);
    protected override EntityProcessor CreatePhysicsProcessor() => new HeadlessPhysicsProcessor();

    protected override ContentManagerLoaderSettings CreateContentManagerLoaderSettings => new ContentManagerLoaderSettings
    {
        // Ignore all references (Model, etc...)
        ContentFilter = ContentManagerLoaderSettings.NewContentFilterByType()
    };

    protected override IGamePlatformEx CreateGamePlatform() => ActivatorUtilities.CreateInstance<ServerGamePlatform>(ServiceProvider);

    public GameSettings Settings { get; set; } = new();

    protected override void OnConstructed()
    {

        #region REVIEW - this is from MultiplayerExample.Game GameAppServer.cs.  Compare with Stride.Games.dll

        // Initialize assets
        //if (_initializeDatabase)
        {
            //DatabaseFileProvider = InitializeAssetDatabase(); // TODO?
            //((DatabaseFileProviderService)_services.GetService<IDatabaseFileProviderService>()).FileProvider = _databaseFileProvider; // TODO?

            if (Content.Exists(GameSettings.AssetUrl))  // TODO: maybe server needs its own GameSettings asset url?
            {
                Settings = Content.Load<GameSettings>(GameSettings.AssetUrl);
            }
            else
            {
                Settings = new GameSettings
                {
                    Configurations = new PlatformConfigurations(),
                };
                //var navSettings = Settings.Configurations.Get<NavigationSettings>();
                //if (navSettings == null)
                //{
                //    var navConfigSettings = new ConfigurationOverride
                //    {
                //        Configuration = navSettings
                //    };
                //    Settings.Configurations.Configurations.Add(navConfigSettings);
                //}
            }
            StrideServices.AddService<IGameSettingsService>(this);
        }
        // Run server at a fixed rate, set manually via _physicGameTime
        var physicsSettings = Settings.Configurations.Get<PhysicsSettings>() ?? new PhysicsSettings();
        physicsSettings.Flags = PhysicsEngineFlags.ContinuousCollisionDetection;
        physicsSettings.FixedTimeStep = (float)StrideServerConfig.PhysicsFixedTimeStep.TotalSeconds;
        physicsSettings.MaxTickDuration = physicsSettings.FixedTimeStep;    // Important to keep this the same as FixedTimeStep since this makes BulletPhysics simulate exactly one step per update
        var physicsConfigSettings = new ConfigurationOverride
        {
            Configuration = physicsSettings
        };
        Settings.Configurations.Configurations.Add(physicsConfigSettings);
        #endregion
    }
    //protected override async Task LoadMainScene()
    //{
    //    Physics = new PhysicsProcessor();
    //    SceneInstance.Processors.Add(Physics);
    //}

    //void Example()
    //{
    //    //var result = Physics.Simulation.Raycast(start, end);
    //    //Console.WriteLine($"Performing raycast: {(result.Succeeded ? "hit" : "miss")}");
    //}
}
