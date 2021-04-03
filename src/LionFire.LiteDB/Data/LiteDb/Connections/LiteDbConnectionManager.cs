using LionFire.Data;
using LionFire.Data.Connections;
using Microsoft.Extensions.Hosting;
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
    public class LiteDbConnectionManagerOptions
    {        
        public bool AutoCreateAll { get; set; }
        public bool AutoConnectAll { get; set; }
    }

    public class LiteDbConnectionManager : ConnectionManager<LiteDbConnection, LiteDbConnectionOptions>, IHostedService
    {

        public LiteDbConnectionManager(IOptionsMonitor<LiteDbConnectionManagerOptions> connectionManagerOptionsMonitor, IOptionsMonitor<NamedConnectionOptions<LiteDbConnectionOptions>> options, ILogger<LiteDbConnectionManager> logger, IServiceProvider serviceProvider) : base(options, logger, serviceProvider)
        {
            ConnectionManagerOptionsMonitor = connectionManagerOptionsMonitor;
        }

        public IOptionsMonitor<LiteDbConnectionManagerOptions> ConnectionManagerOptionsMonitor { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (ConnectionManagerOptionsMonitor.CurrentValue.AutoCreateAll)
            {
                // FUTURE: create all databases
                throw new NotImplementedException();
                if (!ConnectionManagerOptionsMonitor.CurrentValue.AutoConnectAll)
                {
                    // Disconnect to databases that were just created
                    throw new NotImplementedException();
                }
            }
            if (ConnectionManagerOptionsMonitor.CurrentValue.AutoConnectAll)
            {
                // FUTURE: connect to all databases
                throw new NotImplementedException();
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.WhenAll(ConnectionsByConnectionName.Values.Select(c => c.StopAsync(cancellationToken)));
    }
}
