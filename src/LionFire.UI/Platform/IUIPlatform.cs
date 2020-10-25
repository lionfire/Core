using LionFire.Threading;

namespace LionFire.UI
{
    public interface IUIPlatform
    {
        IDispatcher Dispatcher { get; }
    }
}
