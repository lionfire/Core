namespace LionFire.Data.Connections
{
    public abstract class ConnectingConnectionOptions<TConcrete> : ConnectionOptions<TConcrete>, IConnectingConnectionOptions
        where TConcrete : ConnectingConnectionOptions<TConcrete>
    {
        public bool AutoConnect { get; set; }
    }
}
