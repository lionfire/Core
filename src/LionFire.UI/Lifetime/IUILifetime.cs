using System.Threading;

namespace LionFire.UI
{
    public interface IUILifetime
    {
        CancellationToken Started { get; }
        CancellationToken Stopping { get; }
        CancellationToken Stopped { get; }

        void StopUI();
    }
}
