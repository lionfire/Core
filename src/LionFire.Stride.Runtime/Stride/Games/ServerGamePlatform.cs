using LionFire;
using Stride.Core;

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
    [Blocking]
    public void Run(GameContext gameContext)
    {
        throw new NotImplementedException();
    }

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
