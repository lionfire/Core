namespace LionFire.Hosting;

public enum ClusterDiscovery
{
    Unspecified = 0,
    None = 1 << 0,
    Localhost = 1 << 1,
    Consul = 1 << 2,
    Redis = 1 << 3,
}
