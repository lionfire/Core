using LionFire.Data;
using LionFire.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyCouch;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.CouchDB
{

    /// <summary>
    /// Connection string:
    ///   Comma separated host:port
    ///   E.g. "server1:6379,server2:6379"
    ///   Order not important; master is automatically identified
    /// </summary>
    public sealed class CouchDBConnection : ConnectionBase<CouchDBConnectionOptions, CouchDBConnection>
    {
        /// <summary>
        /// Gets or sets the number of milliseconds after which an active System.Net.ServicePoint connection is closed.
        /// </summary>
        public static int ConnectionLeaseTimeout { get; set; } = 60 * 1000; // 1 minute

        public MyCouchClient MyCouchClient { get; private set; }
        MyCouchClient NewCouchClient => new MyCouchClient($"{Options.DatabaseUrl}", Options.Database);

        public CouchDBConnection(CouchDBConnectionOptions options, ILogger<CouchDBConnection> logger) : base(options, logger)
        {
            ConnectionState = ConnectionState.NotConnected;
        }

        //public bool IsConnectionDesired
        //{
        //    get => isConnectionDesired;
        //    set
        //    {
        //        if (value)
        //        {
        //            Connect().FireAndForget();
        //        }
        //        else
        //        {
        //            Disconnect().FireAndForget();
        //        }
        //    }
        //}
        //private bool isConnectionDesired;
        ////private Task<ConnectionMultiplexer> connectingTask;

        public override Task ConnectImpl(CancellationToken cancellationToken = default)
        {
            var sp = System.Net.ServicePointManager.FindServicePoint(new Uri(Options.WebUrl));
            sp.ConnectionLeaseTimeout = ConnectionLeaseTimeout;

            if (MyCouchClient != null)
            {
                this.MyCouchClient = NewCouchClient;
            }
            ConnectionState = ConnectionState.Ready;

            return Task.CompletedTask;
        }
        public override Task DisconnectImpl(CancellationToken cancellationToken = default)
        {
            ConnectionState = ConnectionState.NotConnected;

            MyCouchClient?.Dispose();
            MyCouchClient = null;
            return Task.CompletedTask;
        }
    }
}
