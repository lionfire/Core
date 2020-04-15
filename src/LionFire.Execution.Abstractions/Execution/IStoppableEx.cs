using System.Threading.Tasks;

namespace LionFire.Execution
{
    // DEPRECATED
    public interface IStoppableEx
    {
        Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren);
    }
}
