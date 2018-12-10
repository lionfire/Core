namespace LionFire.ObjectBus
{
    public interface IConnectingOBase : IOBase
    {
        string ConnectionString { get; set; }
    }
}
