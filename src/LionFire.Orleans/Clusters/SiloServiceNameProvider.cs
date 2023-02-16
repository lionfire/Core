using LionFire.Applications;
using Microsoft.Extensions.Options;
using Orleans.Configuration;

namespace LionFire.Hosting;

/// <summary>
/// e.g. silo.infra.services.lionfire.ca
/// </summary>
public class SiloServiceNameProvider
{
    public SiloServiceNameProvider(AppInfo appInfo, IOptions<ClusterOptions> orleansClusterConfig)
    {
        if (!appInfo.IsValid) { throw new InvalidOperationException($"AppInfo is invalid: " + appInfo.InvalidReason); }
        ServiceName = @$"silo.{(
            (orleansClusterConfig.Value.ServiceId).ToLower()
                .Replace("lionfire.", "")
                .Replace(oldValue: "silo.", "")
                .Replace(oldValue: ".silo", "")
            )}.services.{appInfo.OrgDomain}";
    }

    public string ServiceName { get; }
}