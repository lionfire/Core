using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data.Connections
{
    public interface IConnection : IHostedService
    {
        //ILogger Logger { set; }
        //string ConnectionString { get; set; }
        string ConnectionString { get;  } // will all connections have this?


        new Task StartAsync(CancellationToken cancellationToken = default(CancellationToken));
        new Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
