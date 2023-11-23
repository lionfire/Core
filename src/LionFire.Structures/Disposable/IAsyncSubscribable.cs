#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Structures
{
    public interface IAsyncSubscribable
    {
        Task<IDisposable> SubscribeAsync(CancellationToken cancellationToken = default);
    }
}
