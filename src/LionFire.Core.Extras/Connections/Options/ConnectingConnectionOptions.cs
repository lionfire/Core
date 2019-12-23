namespace LionFire.Data
{
    public interface IConnectingConnectionOptions
    {
        bool AutoConnect { get; set; }

    }
    public class ConnectingConnectionOptions<TConcrete> : ConnectionOptions<TConcrete>, IConnectingConnectionOptions
        where TConcrete : ConnectingConnectionOptions<TConcrete>
    {
        public bool AutoConnect { get; set; }
    }
}
