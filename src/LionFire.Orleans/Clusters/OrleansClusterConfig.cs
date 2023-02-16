using Orleans.Serialization.TypeSystem;

namespace LionFire.Hosting;

public class OrleansClusterConfig
{

    public ClusterDiscovery? Kind { get; set; } = ClusterDiscovery.Localhost;

    #region Silo

    /// <summary>
    /// Used for blue/green deployment.  Example: LionFire.MySilo#Blue LionFire.MySilo#Green
    /// </summary>
    /// <remarks>
    /// See https://github.com/dotnet/orleans/issues/5696#issuecomment-503595998
    /// </remarks>
    public string ClusterId { get; set; }

    /// <summary>
    /// Used for storage
    /// </summary>
    /// <remarks>
    /// See https://github.com/dotnet/orleans/issues/5696#issuecomment-503595998
    /// </remarks>
    public string ServiceId { get; set; }

    #endregion

    ///// <summary>
    ///// Name to use when registering the Silo service with a registry like Consul. (Can be overridden in Consul.)
    ///// </summary>
    //public string SiloServiceName { get; set; }

}
