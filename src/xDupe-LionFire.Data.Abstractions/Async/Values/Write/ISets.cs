using LionFire.Results;

namespace LionFire.Data.Sets;

public interface ISets // RENAME to ICommits?
{
    Task<ITransferResult> Set(CancellationToken cancellationToken = default);
}

