
namespace LionFire.Data.Sets;

public interface ISets
{
    Task<ITransferResult> Set(CancellationToken cancellationToken = default);
}

