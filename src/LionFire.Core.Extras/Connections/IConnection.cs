using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data
{
    public interface IConnection : IHostedService
    {
        ILogger Logger { set; }
        string ConnectionString { get; set; }

        new Task StartAsync(CancellationToken cancellationToken = default(CancellationToken));
        new Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
