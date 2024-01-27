

namespace Stride.Games;

public interface IGamePlatformEx : IGamePlatform
{
    void Run(GameContext gameContext);
    void Exit();

    bool IsBlockingRun { get; }

}
