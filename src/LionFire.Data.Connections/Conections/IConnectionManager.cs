﻿//using MorseCode.ITask;

using LionFire.Data.Connections;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data
{
    public interface IConnectionManager
    {

    }
    public interface IConnectionManager<TConnection>
        where TConnection : IConnection
    {
        TConnection this[string connectionName] { get; }
    }

}
namespace LionFire
{
    using LionFire.Data;
    using LionFire.Data.Connections;

    public class ConnectionManagerConstants
    {
        public const string DefaultConnectionName = "default";
    }

    // TODO - move this to an interface default method?
    public static class IConnectionManagerExtensions
    {
        public static async Task<TConnection> GetConnection<TConnection>(this IConnectionManager<TConnection> connectionManager, string name = ConnectionManagerConstants.DefaultConnectionName, CancellationToken cancellationToken = default)
            where TConnection : IConnection
        {
            var connection = connectionManager[name];
            await connection.StartAsync(cancellationToken);
            return connection;
        }
    }
}
