using LionFire.Data.Connections;
using Marten;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace LionFire.Data.Marten.Connections
{
    // var store = DocumentStore.For("host=localhost;database=marten_testing;password=mypassword;username=someuser");

    public class MartenConnection : OptionsConnectionBase<MartenConnectionOptions, MartenConnection>
    {
        public DocumentStore DB => db;
        private DocumentStore db;

        public MartenConnection(string connectionName, IOptionsMonitor<NamedConnectionOptions<MartenConnectionOptions>> options, ILogger<MartenConnection> logger) : base(connectionName, options, logger) { }

        public override Task ConnectImpl(CancellationToken cancellationToken = default)
        {

            if (db != null) return Task.CompletedTask;

            return Task.Run(() =>
            {
                if (Options == null) throw new ArgumentNullException(nameof(Options));
                db = DocumentStore.For(Options.ConnectionString);
            }, cancellationToken);
        }

        public override Task DisconnectImpl(CancellationToken cancellationToken = default)
        {
            var dbCopy = db;
            db = null;
            return Task.Run(() =>
            {
                dbCopy.Dispose();
            }, cancellationToken);
        }
    }
}
