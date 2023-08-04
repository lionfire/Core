
namespace LionFire.Data.Async.Sets;

public interface ISetter
{
    Task<ISetResult> Set(CancellationToken cancellationToken = default);
}

