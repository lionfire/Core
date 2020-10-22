#nullable enable
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface ITryStartable
    {
        /// <returns>Failure information, or null if success</returns>
        Task<object?> TryStartAsync(CancellationToken cancellationToken = default);
    }

}
