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

namespace LionFire.Data.Marten.Connections
{

    public class MartenConnectionManagerOptions
    {
        public bool EagerInitialization { get; set; }   // TODO - do the same as Marten does in .AddMarten
        //public bool AutoCreateAll { get; set; }
        //public bool AutoConnectAll { get; set; }
    }

    public class MartenConnectionManager : ConnectionManager<MartenConnection, MartenConnectionOptions>, IHostedService
    {

        public MartenConnectionManager(IOptionsMonitor<MartenConnectionManagerOptions> connectionManagerOptionsMonitor, IOptionsMonitor<NamedConnectionOptions<MartenConnectionOptions>> options, ILogger<MartenConnectionManager> logger, IServiceProvider serviceProvider) : base(options, logger, serviceProvider)
        {
            ConnectionManagerOptionsMonitor = connectionManagerOptionsMonitor;
        }

        public IOptionsMonitor<MartenConnectionManagerOptions> ConnectionManagerOptionsMonitor { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (ConnectionManagerOptionsMonitor.CurrentValue.EagerInitialization)
            {

            }
            //if (ConnectionManagerOptionsMonitor.CurrentValue.AutoCreateAll)
            //{
            //    // FUTURE: create all databases
            //    throw new NotImplementedException();
            //    if (!ConnectionManagerOptionsMonitor.CurrentValue.AutoConnectAll)
            //    {
            //        // Disconnect to databases that were just created
            //        throw new NotImplementedException();
            //    }
            //}
            //if (ConnectionManagerOptionsMonitor.CurrentValue.AutoConnectAll)
            //{
            //    // FUTURE: connect to all databases
            //    throw new NotImplementedException();
            //}
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.WhenAll(ConnectionsByConnectionName.Values.Select(c => c.StopAsync(cancellationToken)));
    }
}
