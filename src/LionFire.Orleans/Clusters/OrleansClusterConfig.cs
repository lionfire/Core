using System.Reflection;

namespace LionFire.Hosting;

public class OrleansClusterConfig
{
    #region (static)

    //public const string DefaultClusterId = "(Insert ClusterId here)";
    public static string DefaultClusterId => "blue";
    public static string DefaultServiceId
    {
        get
        {
            var result = Assembly.GetEntryAssembly().FullName;
            result = result.Substring(0, result.IndexOf(','));
            result = result.Replace(".Silo", "");
            return result;
        }
    }

    #endregion

    //public OrleansClusterConfig() : this(DefaultServiceId, DefaultClusterId) { }
    //public OrleansClusterConfig(string serviceId, string? clusterId = null)
    //{
    //    ClusterId = clusterId ?? DefaultClusterId;
    //    ServiceId = serviceId ?? DefaultServiceId;
    //}
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
