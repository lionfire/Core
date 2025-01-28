using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Resilience;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Polly;
using Polly.Retry;
using LionFire.Deployment;
using System.Reflection;

namespace LionFire.Hosting;

public static class OrleansClientResilience
{
    public const string RetryTimeoutPolicyKey = "ClusterClient-Retry-Timeout";
}

public static class OrleansClientResilienceX
{
    public static IServiceCollection AddOrleansClientResilience(this IServiceCollection s) => s
      .AddResilienceEnricher() // For Telemetry
      .AddResiliencePipeline(OrleansClientResilience.RetryTimeoutPolicyKey, static builder =>
      {
          // See: https://www.pollydocs.org/strategies/retry.html
          builder.AddRetry(new RetryStrategyOptions
          {
              ShouldHandle = new PredicateBuilder().Handle<IOException>(),
              BackoffType = DelayBackoffType.Exponential,
              Delay = TimeSpan.FromMilliseconds(75),
              MaxDelay = TimeSpan.FromSeconds(7),
              MaxRetryAttempts = 9,
          });

          // See: https://www.pollydocs.org/strategies/timeout.html
          builder.AddTimeout(TimeSpan.FromSeconds(3.0));
      })
  ;
}

public static class OrleansServiceIdX
{
    public static string ClusterConfigKey => "Orleans:Cluster"; // ENH: Ability to configure multiple clusters


    public static string ServiceIdWithReleaseChannelSuffix(IConfiguration configuration)
    {
        var clusterConfig = new OrleansClusterConfig();
        configuration.GetSection(ClusterConfigKey).Bind(clusterConfig);
        var ClusterConfig = clusterConfig;

        if (ClusterConfig.ServiceId != null) return ClusterConfig.ServiceId;

        var result = ClusterConfig.BaseServiceId;
        if (result == null)
        {
            result = Assembly.GetEntryAssembly()?.FullName ?? throw new NotSupportedException($"{nameof(ClusterConfig.ServiceId)} is not available because Assembly.GetEntryAssembly()?.FullName returned null. Set manually: {ClusterConfigKey}:ServiceId");
            result = result.Substring(0, result.IndexOf(','));
            result = result.Replace(".Silo", "");
        }
        var releaseChannel = configuration["releaseChannel"] ?? DefaultReleaseChannels.Prod.Id;

        if (!string.IsNullOrWhiteSpace(releaseChannel) && releaseChannel != DefaultReleaseChannels.Prod.Id)
        {
            result += "-" + releaseChannel;
        }
        return result;
    }

}

public static class OrleansClientServiceCollectionExtensions
{
    public static IHostApplicationBuilder UseOrleansClient_LF(this IHostApplicationBuilder hostBuilder, string? clusterName = null)
    {
        //if (clusterName != null) throw new NotImplementedException("TODO: Support multiple clusters using .NET 8 Keyed Services, once Orleans supports it, or I implement it");

        hostBuilder.Services
            .AddOrleansClientResilience()
            .AddOrleansClient(builder =>
        {
            var configKey = string.IsNullOrWhiteSpace(clusterName) ? "Orleans:Cluster" : "Orleans:Clusters:" + clusterName;

            var serviceId = hostBuilder.Configuration.GetSection(configKey)["ServiceId"];
            if (string.IsNullOrWhiteSpace(serviceId))
            {
                var s = OrleansServiceIdX.ServiceIdWithReleaseChannelSuffix(hostBuilder.Configuration);
                if (s != null)
                {
                    hostBuilder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { $"{configKey}:ServiceId", s }
                    });
                }
            }

            builder.Configure<ClusterOptions>(hostBuilder.Configuration.GetSection(configKey));

            var ClusterConfig = new OrleansClusterConfig();
            hostBuilder.Configuration.GetSection(configKey).Bind(ClusterConfig);

            var ConsulClusterConfig = new OrleansConsulClusterConfig();
            hostBuilder.Configuration.GetSection(configKey + ":Consul").Bind(ConsulClusterConfig);

            switch (ClusterConfig.Kind)
            {
                case ClusterDiscovery.Unspecified:
                    throw new ArgumentException($"Missing Configuration: {configKey}:Kind");
                case ClusterDiscovery.Localhost:
                    builder.UseLocalhostClustering();
                    break;
                case ClusterDiscovery.Consul:
                    builder.UseConsulClientClustering(options =>
                    {
                        options.ConfigureConsulClient(new Uri(ConsulClusterConfig.ServiceDiscoverEndPoint ?? throw new ArgumentNullException($"Missing config: {configKey}:Consul:ServiceDiscoverEndPoint")), ConsulClusterConfig.ServiceDiscoveryToken);
                        options.KvRootFolder = ConsulClusterConfig.KvFolderName ?? ClusterConfig.ServiceId;
                    });
                    break;
                case ClusterDiscovery.None: // FUTURE - support disabling the client here? For now fall through to default which is invalid.  
                default:
                    throw new NotSupportedException($"Unsupported Configuration value for: {configKey}:Kind: {ClusterConfig.Kind}");
            }
        });

        return hostBuilder;
    }
    //public static IServiceCollection AddOrleansClient(this IServiceCollection services, IConfiguration configuration)
    //{
    //    services
    //        .Configure<ClusterOptions>(o =>
    //        {
    //            o.ClusterId = ClusterConfig.ClusterId;
    //            o.ServiceId = ClusterConfig.ServiceId;
    //        })
    //    //    .Configure<OrleansClusterConfig>(configuration.GetSection("Orleans:Cluster"))
    //    //    .Configure<OrleansConsulClusterConfig>(configuration.GetSection("Orleans:Cluster:Consul"))
    //    //;
    //    // OLD - Orleans 3
    //    //.AddSingleton<ClusterClientHostedService>()
    //    //.AddHostedService<ClusterClientHostedService>(sp => sp.GetRequiredService<ClusterClientHostedService>())
    //    //.AddSingleton<IClusterClient>(sp => sp.GetRequiredService<ClusterClientHostedService>().Client);
    //    //.AddSingleton<IGrainFactory>(sp => sp.GetRequiredService<ClusterClientHostedService>().Client);
    //    //;


    //    return services;
    //}
}
