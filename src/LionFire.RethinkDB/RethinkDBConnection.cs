using LionFire.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
using System.Threading;
using System.Threading.Tasks;
using static RethinkDb.Driver.RethinkDB;

namespace LionFire.RethinkDB
{
    public class RethinkDBConnection : ConnectionBase<RethinkDBOptions, RethinkDBConnection>
    {
        //public RethinkDBOptions Options => options.CurrentValue;
        //IOptionsMonitor<RethinkDBOptions> options;

        public RethinkDBConnection(ILogger<RethinkDBConnection> logger, IOptionsMonitor<RethinkDBOptions> options) : base(options.CurrentValue, logger) {
        }

        public Connection Connection => connection;
        private Connection connection;

        public override async Task ConnectImpl(CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(() =>
            {
                var o = Options;

                connection = R.Connection()
                                         .Hostname(o.Host)
                                         .Port(o.Port)
                                         .Timeout(o.TimeoutSeconds)
                                         .Connect();
            }, cancellationToken);
        }
        public override async Task DisconnectImpl(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connection != null)
            {
                var connectionCopy = connection;
                connection = null;

                bool closed = false;
                try
                {
                    await Task.Run(() =>
                      {
                          connectionCopy.Close();
                          closed = true;
                      }, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Exception during graceful disconnect");
                }

                if (!closed)
                {
                    try
                    {
                        await Task.Run(() =>
                        {
                            connectionCopy.Close(false);
                        });
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Exception during forced disconnect");
                    }
                }
            }
        }
    }

}
