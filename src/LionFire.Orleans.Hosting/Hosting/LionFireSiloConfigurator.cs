#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;
using Orleans.Serialization;
using Microsoft.Extensions.Options;
using LionFire.Net;

namespace LionFire.Hosting;

// REVIEW: Streamline this with SiloProgramConfig somehow?
public class LionFireSiloConfigurator
{
    #region Dependencies

    //public IOptions<OrleansClusterConfig> Options { get; }

    public string ClusterConfigKey => "Orleans:Cluster"; // ENH: Ability to configure multiple clusters
    OrleansClusterConfig ClusterConfig { get; }
    public IConfiguration Configuration { get; }

    #region Derived

    //public OrleansClusterConfig ClusterConfig => Options.Value;

    #endregion

    #endregion

    #region Lifecycle

    public LionFireSiloConfigurator(IConfiguration configuration)
    {
        Configuration = configuration;
        //Options = options;

        var clusterConfig = new OrleansClusterConfig();
        configuration.GetSection(ClusterConfigKey).Bind(clusterConfig);
        ClusterConfig = clusterConfig;
    }

    #endregion

    public ClusterDiscovery? Kind => ClusterConfig.Kind;

    #region ClusterId

    public static string DefaultClusterId2 = "default";

    public string ClusterId => ClusterConfig.ClusterId ?? Configuration["slot"] ?? DefaultClusterId2;
    //            if (deploymentId == "blue" || deploymentId == "green") { deploymentId = "prod"; }
    //            if (deploymentId == "beta.blue" || deploymentId == "beta.green") { deploymentId = "beta"; }

    #endregion

    #region ServiceId

    public string ServiceId
    {
        get
        {
            if(ClusterConfig.ServiceId != null) return ClusterConfig.ServiceId;

            var result = Assembly.GetEntryAssembly()?.FullName ?? throw new NotSupportedException($"{nameof(ServiceId)} is not available because Assembly.GetEntryAssembly()?.FullName returned null. Set manually: {ClusterConfigKey}:ServiceId");
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

    #endregion
}

public static class LionFireSiloConfiguratorX
{
    public static IHostBuilder UseLionFireOrleans(this IHostBuilder builder, Action<HostBuilderContext, ISiloBuilder>? configureSilo = null)
    {

        //builder.ConfigureAppConfiguration((context, c) =>
        //{
        // REVIEW - can't modify builder here. still ok?
        //    builder.ConfigureServices((c, s) => s.Configure<ClusterOptions>(o => ConfigureClusterOptions(o, c)));
        //});

        //builder.ConfigureServices(s => s.AddSingleton<IPostConfigureOptions<OrleansJsonSerializerOptions>, ConfigureOrleansJsonSerializerOptions>());

        return builder.UseOrleans((context, siloBuilder) => siloBuilder.UseLionFireOrleans(context, configureSilo));
    }

    private static void ConfigureClusterOptions(ClusterOptions options, HostBuilderContext context)
    {
        var clusterConfigProvider = new LionFireSiloConfigurator(context.Configuration);

        options.ClusterId = clusterConfigProvider.ClusterId;
        options.ServiceId = clusterConfigProvider.ServiceId;
    }

    /// <summary>
    /// (Prefer IHostBuilder.UseLionFireOrleans)
    /// </summary>
    /// <param name="siloBuilder"></param>
    /// <param name="config"></param>
    /// <param name="context"></param>
    /// <param name="configureSilo"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static ISiloBuilder UseLionFireOrleans(this ISiloBuilder siloBuilder, HostBuilderContext context, Action<HostBuilderContext, ISiloBuilder>? configureSilo = null)
    {
        var config = new SiloProgramConfig(context.Configuration);

        var clusterConfigSection = context.Configuration.GetSection("Orleans:Cluster");

        siloBuilder.Configure<ClusterOptions>(o => ConfigureClusterOptions(o, context)); // REVIEW - is this needed, or does the IHostBuilder.ConfigureAppConfiguration cover this? Or is that one redundant?  Or is it needed elsewhere by me?

        var clusterConfigProvider = new LionFireSiloConfigurator(context.Configuration);

        siloBuilder // ...

        #region Kind

                 .If(clusterConfigProvider.Kind == ClusterDiscovery.Localhost, s =>
                     // NOTE - redundant specification of ClusterId and ServiceId
                     s.UseLocalhostClustering(config.SiloPort ?? throw new ArgumentNullException(nameof(config.SiloPort))
                     , config.GatewayPort ?? throw new ArgumentNullException(nameof(config.GatewayPort))
                     , config.LocalhostPrimaryClusterEndpoint ?? throw new ArgumentNullException(nameof(config.LocalhostPrimaryClusterEndpoint))
                     , clusterConfigProvider.ServiceId
                     , clusterConfigProvider.ClusterId
                     ))
                 .If(clusterConfigProvider.Kind == ClusterDiscovery.Consul, s =>
                     s.UseConsulSiloClustering(gatewayOptions =>
                     {
                         OrleansConsulClusterConfig clusterConsulConfig = new();
                         clusterConfigSection.GetSection("Consul").Bind(clusterConsulConfig);
                         //context.Configuration.Bind("Orleans:Cluster:Consul", clusterConsulConfig);

                         if (clusterConsulConfig.ServiceDiscoverEndPoint != null)
                         {
                             gatewayOptions.ConfigureConsulClient(new Uri(clusterConsulConfig.ServiceDiscoverEndPoint), clusterConsulConfig.ServiceDiscoveryToken);
                         }

                         gatewayOptions.KvRootFolder = clusterConsulConfig.KvFolderName ?? $"{clusterConfigProvider.ServiceId}";
                     })
                 )
                 .If(clusterConfigProvider.Kind == ClusterDiscovery.Redis, s =>
                     s.UseRedisClustering(gatewayOptions =>
                     {
                         OrleansRedisClusterConfig clusterRedisConfig = new();
                         clusterConfigSection.GetSection("Redis").Bind(clusterRedisConfig);
                         //context.Configuration.Bind("Orleans:Cluster:Redis", clusterRedisConfig);

                         gatewayOptions.Database = clusterRedisConfig.Database ?? 3;
                         gatewayOptions.ConnectionString = clusterRedisConfig.ConnectionString ?? "localhost:6379";
                     })
                 )
                 .If(clusterConfigProvider.Kind != ClusterDiscovery.Localhost && clusterConfigProvider.Kind != ClusterDiscovery.Consul && clusterConfigProvider.Kind != ClusterDiscovery.Redis, s =>
                     throw new NotSupportedException($"Unknown clusterConfigProvider.Kind: {clusterConfigProvider.Kind}"))

        #endregion

                 .ConfigureEndpoints(IPAddress.Parse(config.SiloInterface)
                     , config.SiloPort ?? throw new ArgumentNullException(nameof(config.SiloPort))
                     , config.GatewayPort ?? throw new ArgumentNullException(nameof(config.GatewayPort)))
                 .ConfigureLogging(logging => logging.AddConsole())
                 ;

        if (config.DashboardEnabled)
        {
            siloBuilder.UseDashboard(options => { 
                options.Port = config.DashboardPort ?? throw new ArgumentNullException(nameof(config.DashboardPort)); 
                options.Host = config.DashboardInterface;
                //options.HostSelf = false;
            });
        }

        configureSilo?.Invoke(context, siloBuilder);

        return siloBuilder;
    }

}
