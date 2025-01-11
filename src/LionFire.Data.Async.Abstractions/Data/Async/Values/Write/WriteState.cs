
namespace LionFire.Data.Async.Sets;

[Flags]
public enum WriteState
{
    Unspecified = 0,
    NoWritesPending = 1 << 0,
    NonOptimisticWritesPending = 1 << 1,
    OptimisticWritesPending = 1 << 2, // Represented in local cache, assumes pending writes will succeed
}
