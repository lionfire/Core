using LionFire.Data;
using LionFire.Data.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
//using IConnection = LionFire.Data.IConnection;

namespace LionFire.RethinkDB;

/// <summary>
/// Returns a RethinkConnection (wraps ConnectionMultiplexer) based on a Configuration key for a connection string.  Will reuse connections that match the same
/// connection string.
/// </summary>
public class RethinkDBConnectionManager : ConnectionManager<RethinkDBConnection, RethinkDBOptions>
{
    public RethinkDBConnectionManager(IOptionsMonitor<NamedConnectionOptions<RethinkDBOptions>> optionsMonitor, ILogger<RethinkDBConnectionManager> logger, IServiceProvider serviceProvider) 
        : base(optionsMonitor, logger, serviceProvider)
    {
    }
}
