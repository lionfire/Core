using LionFire.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Redis
{
    /// <summary>
    /// FUTURE: retry logic
    /// FUTURE: state machine
    /// FUTURE: Health monitoring
    /// </summary>
    public abstract class ConnectionBase : IHostedService, IConnection
    {

        public ILogger Logger { set => this.logger = value; }
        protected ILogger logger;

        public string ConnectionString { get; set; }

        public abstract Task Connect(CancellationToken cancellationToken = default(CancellationToken));
        public abstract Task Disconnect(CancellationToken cancellationToken = default(CancellationToken));

        #region IHostedService

        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken)) => await Connect(cancellationToken);

        public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken)) => await Disconnect(cancellationToken);

        #endregion
    }
}
