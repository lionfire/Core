using LionFire.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using IConnection = LionFire.Data.IConnection;

namespace LionFire.RethinkDB
{
    /// <summary>
    /// Returns a RethinkConnection (wraps ConnectionMultiplexer) based on a Configuration key for a connection string.  Will reuse connections that match the same
    /// connection string.
    /// </summary>
    public class RethinkDBConnectionManager : ConnectionManagerBase<RethinkDBConnection>
    {
        public RethinkDBConnectionManager(IConfiguration configuration, ILogger<RethinkDBConnectionManager> logger, IServiceProvider serviceProvider) : base(configuration, logger, serviceProvider)
        {
        }
    }

}
