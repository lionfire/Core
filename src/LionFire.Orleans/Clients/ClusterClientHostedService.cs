using LionFire.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Orleans_.Clients
{
    public class ClusterClientHostedService : IHostedService
    {
        public IClusterClient Client { get; }

        public ClusterClientHostedService(ILoggerProvider loggerProvider, IOptions<OrleansClusterConfig> clusterConfig, IOptions<OrleansConsulClusterConfig> consulClusterConfig)
        {
            IHostBuilder hb;
            hb.UseOrleansClient(builder =>
            {
                OrleansClusterConfig ClusterConfig;
                OrleansConsulClusterConfig ConsulClusterConfig;

                ClusterConfig = clusterConfig.Value;
                ConsulClusterConfig = consulClusterConfig.Value;

                //var builder = new ClientBuilder();

                switch (ClusterConfig.Kind)
                {
                    case ClusterDiscovery.Unspecified:
                        break;
                    case ClusterDiscovery.None:
                        break;
                    case ClusterDiscovery.Localhost:
                        builder.UseLocalhostClustering();
                        break;
                    case ClusterDiscovery.Consul:
                        builder.UseConsulClientClustering(gatewayOptions =>
                        {
                            Consul.IConsulClient c;
                            c.
                            gatewayOptions.ConfigureConsulClient(consulClient =>
                            {
                                consulClient.
                            });
                            //OrleansConsulClusterConfig clusterConsulConfig = new();
                            //context.Configuration.Bind("Orleans:Cluster:Consul", ConsulClusterConfig);

                            if (ConsulClusterConfig.ServiceDiscoverEndPoint != null)
                            {
                                gatewayOptions.Address = new Uri(ConsulClusterConfig.ServiceDiscoverEndPoint);
                            }
                            gatewayOptions.AclClientToken = ConsulClusterConfig.ServiceDiscoveryToken;
                            gatewayOptions.KvRootFolder = ConsulClusterConfig.KvFolderName ?? ClusterConfig.ServiceId;

                        });
                        break;
                    default:
                        break;
                }

            });

            Client = builder
                .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
                .Configure<ClusterOptions>(o =>
                {
                    o.ClusterId = ClusterConfig.ClusterId;
                    o.ServiceId = ClusterConfig.ServiceId;
                })
                .Build();

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            // A retry filter could be provided here.
            await Client.Connect();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Client.Close();

            Client.Dispose();
        }
    }
}

