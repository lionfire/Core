using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using LionFire.Data;

namespace LionFire.CouchDB
{
    /// <summary>
    /// Returns a CouchDBConnection (wraps ConnectionMultiplexer) based on a Configuration key for a connection string.  Will reuse connections that match the same
    /// connection string.
    /// </summary>
    public class CouchDBConnectionManager : ConnectionManagerBase<CouchDBConnection>
    {
        
        public CouchDBConnectionManager(IConfiguration configuration, ILogger<CouchDBConnectionManager> logger, IServiceProvider serviceProvider) : base(configuration, logger, serviceProvider)
        {
        }
    }
}
