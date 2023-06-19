using LionFire.Results;

namespace LionFire.Data.Async.Sets;

public interface ISets
{
    Task<ITransferResult> Set(CancellationToken cancellationToken = default);
}

