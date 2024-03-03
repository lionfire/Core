

namespace Stride.Games;

public interface IGamePlatformEx : IGamePlatform
{
    void Run(GameContext gameContext);
    void Exit();

    bool IsBlockingRun { get; }

    /// <summary>
    /// The Server or Window main loop repeatedly invokes this.
    /// </summary>
    public Action RunCallback { get; set; }

}
