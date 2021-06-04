using LionFire.Data.Connections;
using LiteDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace LionFire.Data.LiteDB.Connections
{
    public class LiteDbConnection : OptionsConnectionBase<LiteDbConnectionOptions, LiteDbConnection>
    {
        public LiteDatabase DB => db;
        private LiteDatabase db;

        public LiteDbConnection(string connectionName, IOptionsMonitor<NamedConnectionOptions<LiteDbConnectionOptions>> options, ILogger<LiteDbConnection> logger) : base(connectionName, options, logger) { }

        public override Task ConnectImpl(CancellationToken cancellationToken = default)
        {
            if (db != null) return Task.CompletedTask;

            return Task.Run(() =>
            {
                if (Options == null) throw new ArgumentNullException(nameof(Options));
                db = new LiteDatabase(Options.ConnectionString);
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
