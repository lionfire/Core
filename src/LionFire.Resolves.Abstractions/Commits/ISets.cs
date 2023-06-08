using LionFire.Results;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

public interface ISets
{
    Task<ISuccessResult> Set(CancellationToken cancellationToken = default);
}

