using LionFire.Stride3D;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stride.Core;
using Stride.Core.Diagnostics;
using Stride.Core.IO;
using Stride.Core.Serialization.Contents;
using Stride.Core.Storage;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Engine.Network;
using Stride.Games;
using Stride.Physics;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Stride3D;


public static class GameX
{
    public static Simulation Simulation(this Game game) => game.SceneSystem.SceneInstance.GetProcessor<PhysicsProcessor>()?.Simulation;
}

/// <summary>
/// TODO:
/// - 
/// </summary>
public class StrideGameService : IHostedService
{
   

    #region Dependencies

    public IHostApplicationLifetime HostApplicationLifetime { get; }
    public ILogger<StrideGameService> Logger { get; }

    #endregion

    #region Lifecycle

    public StrideGameService(IHostApplicationLifetime hostApplicationLifetime, ILogger<StrideGameService> logger)
    {
        HostApplicationLifetime = hostApplicationLifetime;
        Logger = logger;
        GlobalLogger.GlobalMessageLogged += GlobalLogger_GlobalMessageLogged;

    }

    #endregion

    #region State

    public Game Game => game;
    private Game game;

    Task strideGameRunTask;

    #region Derived

    public Simulation Simulation => game.Simulation(); // game.SceneSystem.SceneInstance.GetProcessor<PhysicsProcessor>()?.Simulation;

    #endregion

    #endregion

    #region IHostedService

    public Task StartAsync(CancellationToken cancellationToken)
    {
        strideGameRunTask = Task.Run(() =>
        {
            using (game = new Game())
            {
                game.ConsoleLogLevel = LogMessageType.Debug;
                game.ConsoleLogMode = ConsoleLogMode.Always;
                Task.Run(async () =>
                {
                    while (game?.Window == null)
                    {
                        Logger.LogDebug("Waiting for Game.Window to appear...");
                        await Task.Delay(200);
                    }
                    Logger.LogDebug("Waiting for Game window to appear...done.");
                    game.Window.Closing += Window_Closing;
                });
                game.Run();
            }
        });

        strideGameRunTask.ContinueWith(_ => OnGameFinished());
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        game.Exit();
        while (game.IsRunning)
        {
            await Task.Delay(100).ConfigureAwait(false);
        }
    }

    #endregion

    #region Handlers

    void OnGameFinished()
    {
        Logger.LogInformation("Stride Game finished.");
    }

    private void GlobalLogger_GlobalMessageLogged(ILogMessage obj)
    {
        Logger.LogInformation("[stride] [module: {module}] {message}", obj.Module, obj.Text);
    }

    private void Window_Closing(object sender, EventArgs e)
    {
        Logger.LogInformation($"Received Window Closing event from Stride.  Stopping Application.");
        HostApplicationLifetime.StopApplication();
    }

    #endregion

#if TRIAGE // TODO - was WIP?
    ConcurrentDictionary<SceneInstance, StrideServiceRegistry> sceneRegistries = new ConcurrentDictionary<SceneInstance, StrideServiceRegistry>();

    //public IServiceProvider GetServiceProvider(SceneInstance sceneInstance = null)
    //{
    //    if (sceneInstance != null)
    //    {
    //        // FUTURE: different IServiceProvider for non-default sceneInstance
    //        throw new NotImplementedException();
    //    }

    //    sceneRegistries.GetOrAdd(sceneInstance, s => new StrideSceneRegistry(s));
    //    Game.SceneSystem.SceneInstance.Services

    //}
#endif


#if UNUSED
    #region DataContext

    public object DataContext
    {
        get => dataContext;
        set
        {
            if (dataContext == value) return;
            if (dataContext != null && value != null) throw new AlreadySetException("Must first set back to null");
            dataContext = value;
        }
    }
    private object dataContext;

    #endregion
#endif
}
