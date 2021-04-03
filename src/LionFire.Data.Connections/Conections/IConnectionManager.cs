//using MorseCode.ITask;

using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data.Connections
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
    using LionFire.Data.Connections;

    // TO.NET5 - move this to an interface default method
    public static class IConnectionManagerExtensions
    {
        public static async Task<TConnection> GetConnection<TConnection>(this IConnectionManager<TConnection> connectionManager, string name, CancellationToken cancellationToken = default)
            where TConnection : IConnection
        {
            var connection = connectionManager[name];
            await connection.StartAsync(cancellationToken);
            return connection;
        }
    }
}
