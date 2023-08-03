
namespace LionFire.Data.Async.Sets;

public interface ISetter
{
    Task<ITransferResult> Set(CancellationToken cancellationToken = default);
}

