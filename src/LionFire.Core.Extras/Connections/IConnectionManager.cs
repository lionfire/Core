//using MorseCode.ITask;

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

    public static class IConnectionManagerExtensions
    {
        public static TConnection GetConnection<TConnection>(this IConnectionManager<TConnection> connectionManager, string name)
            where TConnection : IConnection
            => connectionManager[name];

        public static async Task<TConnection> StartConnection<TConnection>(this IConnectionManager<TConnection> connectionManager, string name, CancellationToken cancellationToken = default)
            where TConnection : IConnection
        {
            var connection = connectionManager[name];
            await connection.StartAsync(cancellationToken);
            return connection;
        }
    }
}
