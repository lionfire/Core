#nullable enable
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public interface ITryStoppable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Failure information, or null if success</returns>
        Task<object?> TryStopAsync(CancellationToken cancellationToken);
    }

}
