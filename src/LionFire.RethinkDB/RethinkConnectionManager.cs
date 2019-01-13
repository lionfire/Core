using Microsoft.Extensions.Options;
using RethinkDb.Driver.Net;
using RethinkDb.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace LionFire.Rethink
{
    /// <summary>
    /// Returns a RethinkConnection (wraps ConnectionMultiplexer) based on a Configuration key for a connection string.  Will reuse connections that match the same
    /// connection string.
    /// </summary>
    public class RethinkConnectionManager : ConnectionManagerBase<RethinkConnection>
    {
        public RethinkConnectionManager(IConfiguration configuration, ILogger<RethinkConnectionManager> logger, IServiceProvider serviceProvider) : base(configuration, logger, serviceProvider)
        {
        }
    }

    public class RethinkConnection
    {
        public RethinkOptions Options { get; private set; }

        public RethinkConnection(RethinkOptions Options)
        {
            this.Options = Options;
        }


        Connection Connection
        {
            get
            {
                if (connection == null || !connection.Open)
                {
                    Connect();
                }

            }
        }
        Connection connection;

        private void Connect()
        {
            var o = options.CurrentValue;

            connection = R.Connection()
             .Hostname(o.Host)
             .Port(o.Port)
             .Timeout(o.TimeoutSeconds)
             .Connect();
        }
    }
    
}
