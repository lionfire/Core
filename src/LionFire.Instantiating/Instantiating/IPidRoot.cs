
using LionFire.Persistence;
using LionFire.Structures;

namespace LionFire.Instantiating
{
    public interface IPidRoot : INotifyOnSaving
    {
        KeyKeeperShort KeyKeeper { get; }
    }
}
