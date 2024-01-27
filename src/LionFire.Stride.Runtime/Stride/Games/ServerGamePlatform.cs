using LionFire;
using Stride.Core;
using System.Threading;

namespace Stride.Games;

public class ServerGamePlatform : ReferenceBase, IGamePlatformEx
//, IGraphicsDeviceFactory
{
    #region Dependencies

    public ILogger<ServerGamePlatform> Logger { get; }

    #endregion

    #region Parameters

    public string DefaultAppDirectory => throw new NotImplementedException();

    #endregion

    #region Lifecycle

    public ServerGamePlatform(ILogger<ServerGamePlatform> logger)
    {
        Logger = logger;
    }

    protected override void Destroy()
    {
    }

    public bool IsBlockingRun { get; set; } = true;

    internal Action InitCallback;

    public Action RunCallback;

    internal Action ExitCallback;
    internal bool Exiting;

    [Blocking]
    public async void Run(GameContext gameContext)
    {
        // Initialize the init callback
        //InitCallback();

        var context = gameContext;
        if (context.IsUserManagingRun)
        {
            context.RunCallback = RunCallback;
            context.ExitCallback = ExitCallback;
        }
        else
        {
            try
            {
                while (true)
                {
                    await PeriodicTimer.WaitForNextTickAsync();
                    if (Exiting)
                    {
                        Destroy();
                        return;
                    }

                    RunCallback();
                }
            }
            finally
            {
                ExitCallback?.Invoke();
            }
        }
    }
    PeriodicTimer PeriodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(1.0));

    public void Exit()
    {
    }
    #endregion

    #region Stubs for headless

    public GameWindow MainWindow => null;


    public GameWindow CreateWindow(GameContext gameContext = null)
    {
        return null;
    }

    #endregion

}
