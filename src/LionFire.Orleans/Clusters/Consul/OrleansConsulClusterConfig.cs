namespace LionFire.Hosting;

public class OrleansConsulClusterConfig
{
    public string ServiceDiscoverEndPoint { get; set; } = "http://localhost:8500";
    public string ServiceDiscoveryToken { get; set; }

    /// <summary>
    /// Top level prefix.
    /// If null, it will default to ServiceName.
    /// Note: orleans is also downstream in addition to whatever is here:
    /// </summary>
    public string KvFolderName { get; set; } 
}
