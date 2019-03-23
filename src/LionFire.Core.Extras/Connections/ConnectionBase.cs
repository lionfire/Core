using LionFire.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data
{
    /// <summary>
    /// FUTURE: retry logic
    /// FUTURE: state machine
    /// FUTURE: Health monitoring
    /// </summary>
    public abstract class ConnectionBase : IHostedService, IConnection
    {
        protected ILogger logger;

        #region ConnectionString

        /// <summary>
        /// Set by connection manager when creating the connection
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
            set
            {
                if (connectionString == value) return;
                if (connectionString != default(string)) throw new AlreadySetException();
                connectionString = value;
            }
        }
        private string connectionString;

        #endregion

        protected int connectionCount = 0;

        public ConnectionBase(ILogger logger)
        {
            this.logger = logger;
        }

        public abstract Task Connect(CancellationToken cancellationToken = default(CancellationToken));
        public abstract Task Disconnect(CancellationToken cancellationToken = default(CancellationToken));

        #region IHostedService

        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connectionCount++ == 0)
            {
                try
                {
                    await Connect(cancellationToken);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Exception when attempting to connect");
                    throw;
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (--connectionCount <= 0)
            {
                connectionCount = 0;
                await Disconnect(cancellationToken);
            }
        }

        #endregion
    }
}
