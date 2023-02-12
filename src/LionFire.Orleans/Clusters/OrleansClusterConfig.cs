using Microsoft.Extensions.Configuration;
using Orleans.Serialization.TypeSystem;
using System.Reflection;

namespace LionFire.Hosting;

public class OrleansClusterConfigProvider
{
    public OrleansClusterConfig ClusterConfig { get; }

    public IConfiguration Configuration { get; }

    public OrleansClusterConfigProvider(IConfiguration configuration)
    {
        Configuration = configuration;
        OrleansClusterConfig clusterConfig = new();
        configuration.Bind("Orleans:Cluster", clusterConfig);
        ClusterConfig = clusterConfig;
    }

    public ClusterDiscovery? Kind => ClusterConfig.Kind;


    public static string DefaultClusterId2 = "blue";


    public string ClusterId => Configuration["slot"] ?? DefaultClusterId2;
    //var deploymentId = clusterId;
    //            if (deploymentId == "blue" || deploymentId == "green") { deploymentId = "prod"; }
    //            if (deploymentId == "beta.blue" || deploymentId == "beta.green") { deploymentId = "beta"; }

    public  string ServiceId
    {
        get
        {
            var result = Assembly.GetEntryAssembly()?.FullName ?? throw new NotSupportedException($"{nameof(ServiceId)} is not available because Assembly.GetEntryAssembly()?.FullName returned null.");
            result = result.Substring(0, result.IndexOf(','));
            result = result.Replace(".Silo", "");

            var releaseChannel = Configuration["releaseChannel"] ?? "prod";

            if (!string.IsNullOrWhiteSpace(releaseChannel) && releaseChannel != "prod")
            {
                result += "-" + releaseChannel;
            }

            return result;
        }
    }
}

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
