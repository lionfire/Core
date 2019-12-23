//using MorseCode.ITask;

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
